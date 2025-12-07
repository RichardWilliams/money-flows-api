# Phase 2B: Core MoneyFlows Feature Implementation

## Context

You are continuing development of the UK Property Management API. This is **Phase 2B** - completing the original Phase 2 plan that was deferred.

**Repository:** https://github.com/RichardWilliams/money-flows-api
**Branch:** `main` (start from here)
**Location:** `/workspace/money-flows/backend`

### What's Already Complete

**Phase 1:** Infrastructure ✅
- .NET 8.0 Web API with Minimal APIs
- PostgreSQL 16 with EF Core 8.0
- CQRS with MediatR
- FluentValidation, AutoMapper
- Serilog structured logging
- Health checks
- TransactionBehavior for automatic transactions
- **Important:** NO retry strategy in DbContext (removed - conflicts with TransactionBehavior)

**Phase 2 (Actually 2A):** Domain Features ✅
- **Properties** - UK properties with postcode validation, types (Detached, Semi-Detached, etc.)
- **Expenses** - 13 UK expense categories (Council Tax, Maintenance, Insurance, etc.), multi-currency
- **Tenants** - UK phone validation, emergency contacts
- **Leases** - Business rules, rent day validation (1-28), status management

**Database:**
- Host: `postgres` (container network)
- Database: `property_mgmt`
- Schema: `property_management`
- 6 tables exist: properties, expenses, expense_categories, expense_attachments, tenants, leases

**Tools Available:**
- `dotnet tool restore` - Installs dotnet-ef 8.0.11
- `dotnet ef` - EF Core CLI tools

## Phase 2B Goal

Implement **MoneyFlows** - the core financial transaction tracking feature that ties all other features together.

### What is MoneyFlows?

MoneyFlows represents ALL financial transactions (income and expenses) across your rental properties:

**Examples:**
- **Income:** Rental payment from tenant, Airbnb booking payout, Deposit received
- **Expense:** Utilities bill, Maintenance work, Council tax payment, Cleaner wages

**Key Concept:** This is different from the "Expenses" feature built in Phase 2A:
- **Expenses feature (Phase 2A):** Categories and categorization system
- **MoneyFlows feature (Phase 2B):** Actual transaction records that USE those categories

### Requirements

#### 1. Domain Model

```csharp
namespace PropertyManagement.Api.Features.MoneyFlows.Domain;

public record MoneyFlow
{
    public Guid Id { get; init; }
    public Guid PropertyId { get; init; }  // Required - FK to properties
    public MoneyFlowType Type { get; init; }  // Income or Expense
    public decimal Amount { get; init; }
    public string Currency { get; init; }  // GBP, CHF, EUR, USD
    public DateOnly Date { get; init; }
    public string Description { get; init; }

    // For Expenses - link to expense category
    public Guid? ExpenseCategoryId { get; init; }

    // For Income - source description
    public string? IncomeSource { get; init; }  // "Rental Income", "Airbnb Booking", etc.

    // Optional relationships
    public Guid? TenantId { get; init; }  // If income from tenant or expense for tenant property
    public Guid? LeaseId { get; init; }  // If related to specific lease

    // Reference numbers
    public string? Reference { get; init; }  // Invoice/receipt number
    public string? Notes { get; init; }

    // Audit
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public enum MoneyFlowType
{
    Income = 1,
    Expense = 2
}

public record MoneyFlowAttachment
{
    public Guid Id { get; init; }
    public Guid MoneyFlowId { get; init; }
    public string FileName { get; init; }
    public string StoragePath { get; init; }  // {year}/{month}/{guid}-{filename}
    public long FileSizeBytes { get; init; }
    public string ContentType { get; init; }
    public DateTime UploadedAt { get; init; }
}
```

#### 2. Features to Implement

Follow the **Vertical Slice Architecture** pattern from Phase 2A:

```
Features/MoneyFlows/
├── Domain/
│   ├── MoneyFlow.cs
│   ├── MoneyFlowType.cs
│   └── MoneyFlowAttachment.cs
├── Application/
│   ├── Commands/
│   │   ├── CreateMoneyFlowCommand.cs
│   │   ├── UpdateMoneyFlowCommand.cs
│   │   ├── DeleteMoneyFlowCommand.cs
│   │   ├── UploadAttachmentCommand.cs (optional for Phase 2B)
│   │   └── DeleteAttachmentCommand.cs (optional for Phase 2B)
│   ├── Queries/
│   │   ├── GetMoneyFlowQuery.cs
│   │   └── ListMoneyFlowsQuery.cs
│   └── Dtos/
│       ├── MoneyFlowDto.cs
│       └── MoneyFlowListDto.cs
├── Infrastructure/
│   └── Persistence/
│       ├── MoneyFlowConfiguration.cs
│       └── MoneyFlowAttachmentConfiguration.cs
└── Endpoints/
    └── MoneyFlowEndpoints.cs
```

**Commands:**
- ✅ `CreateMoneyFlowCommand` - Create transaction (income or expense)
- ✅ `UpdateMoneyFlowCommand` - Update transaction
- ✅ `DeleteMoneyFlowCommand` - Delete transaction
- ⚠️ `UploadAttachmentCommand` - OPTIONAL for now (can defer to Phase 3)
- ⚠️ `DeleteAttachmentCommand` - OPTIONAL for now

**Queries:**
- ✅ `GetMoneyFlowQuery` - Get single transaction with details
- ✅ `ListMoneyFlowsQuery` - List with advanced filtering:
  - Filter by PropertyId
  - Filter by Type (Income/Expense)
  - Filter by Date Range (DateFrom/DateTo)
  - Filter by ExpenseCategoryId
  - Filter by TenantId
  - Search by Description/Reference
  - Pagination (pageNumber, pageSize)
  - Sorting by Date (desc by default)

#### 3. Validation Rules

**CreateMoneyFlowCommand / UpdateMoneyFlowCommand:**
- ✅ PropertyId is required and must exist
- ✅ Type is required (Income or Expense)
- ✅ Amount must be > 0
- ✅ Currency must be one of: GBP, CHF, EUR, USD
- ✅ Date is required, cannot be in future
- ✅ Description is required, min 3 chars, max 500 chars
- ✅ If Type = Expense and ExpenseCategoryId provided, category must exist
- ✅ If TenantId provided, tenant must exist
- ✅ If LeaseId provided, lease must exist
- ⚠️ Reference is optional, max 100 chars
- ⚠️ Notes is optional, max 2000 chars

#### 4. Database Schema

**Table: money_flows**
```sql
CREATE TABLE property_management.money_flows (
    id UUID PRIMARY KEY,
    property_id UUID NOT NULL,
    type INTEGER NOT NULL,  -- 1=Income, 2=Expense
    amount DECIMAL(18,2) NOT NULL,
    currency VARCHAR(3) NOT NULL,
    date DATE NOT NULL,
    description VARCHAR(500) NOT NULL,
    expense_category_id UUID NULL,
    income_source VARCHAR(255) NULL,
    tenant_id UUID NULL,
    lease_id UUID NULL,
    reference VARCHAR(100) NULL,
    notes TEXT NULL,
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP NOT NULL,

    CONSTRAINT fk_money_flows_property FOREIGN KEY (property_id) REFERENCES property_management.properties(id),
    CONSTRAINT fk_money_flows_expense_category FOREIGN KEY (expense_category_id) REFERENCES property_management.expense_categories(id),
    CONSTRAINT fk_money_flows_tenant FOREIGN KEY (tenant_id) REFERENCES property_management.tenants(id),
    CONSTRAINT fk_money_flows_lease FOREIGN KEY (lease_id) REFERENCES property_management.leases(id)
);

CREATE INDEX idx_money_flows_property_date ON property_management.money_flows(property_id, date DESC);
CREATE INDEX idx_money_flows_type ON property_management.money_flows(type);
CREATE INDEX idx_money_flows_date ON property_management.money_flows(date DESC);
CREATE INDEX idx_money_flows_expense_category ON property_management.money_flows(expense_category_id);
CREATE INDEX idx_money_flows_tenant ON property_management.money_flows(tenant_id);
```

**Table: money_flow_attachments** (OPTIONAL - can defer)
```sql
CREATE TABLE property_management.money_flow_attachments (
    id UUID PRIMARY KEY,
    money_flow_id UUID NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    storage_path VARCHAR(500) NOT NULL,
    file_size_bytes BIGINT NOT NULL,
    content_type VARCHAR(100) NOT NULL,
    uploaded_at TIMESTAMP NOT NULL,

    CONSTRAINT fk_money_flow_attachments_money_flow FOREIGN KEY (money_flow_id) REFERENCES property_management.money_flows(id) ON DELETE CASCADE
);

CREATE INDEX idx_money_flow_attachments_money_flow ON property_management.money_flow_attachments(money_flow_id);
```

#### 5. API Endpoints

All under `/api/moneyflows`:

- ✅ `POST /api/moneyflows` - Create money flow
- ✅ `PUT /api/moneyflows/{id}` - Update money flow
- ✅ `GET /api/moneyflows/{id}` - Get money flow details
- ✅ `GET /api/moneyflows` - List money flows with filtering
- ✅ `DELETE /api/moneyflows/{id}` - Delete money flow
- ⚠️ `POST /api/moneyflows/{id}/attachments` - Upload attachment (OPTIONAL)
- ⚠️ `DELETE /api/moneyflows/{id}/attachments/{attachmentId}` - Delete attachment (OPTIONAL)

**Example Request Bodies:**

```json
// Create Income
{
  "propertyId": "b66459e4-e65f-4904-80b9-0b3c7b676686",
  "type": 1,
  "amount": 1500.00,
  "currency": "GBP",
  "date": "2024-12-01",
  "description": "Monthly rental payment",
  "incomeSource": "Rental Income",
  "tenantId": "ef43349e-0387-44e0-9671-c86bb02cad64",
  "leaseId": "c51747b5-8e8a-46d1-9a90-db338a2f48b0",
  "reference": "RENT-DEC-2024"
}

// Create Expense
{
  "propertyId": "b66459e4-e65f-4904-80b9-0b3c7b676686",
  "type": 2,
  "amount": 250.50,
  "currency": "GBP",
  "date": "2024-12-01",
  "description": "Council tax payment for December",
  "expenseCategoryId": "11111111-1111-1111-1111-111111111114",
  "reference": "CT-2024-12",
  "notes": "Paid via direct debit"
}
```

#### 6. File Attachments (OPTIONAL for Phase 2B)

**Can be deferred to Phase 3**, but if implementing:

**Storage Structure:**
```
/app/attachments/moneyflows/
  {year}/
    {month}/
      {guid}-{original-filename}
```

Example: `/app/attachments/moneyflows/2024/12/a1b2c3d4-e5f6-invoice.pdf`

**Use existing settings:**
- `AttachmentSettings.BasePath` = `/app/attachments`
- `AttachmentSettings.MaxFileSizeMb` = 10
- `AttachmentSettings.AllowedExtensions` = [".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx", ".xls", ".xlsx"]

## Implementation Steps

### Step 1: Create Domain Models
- `MoneyFlow.cs` record
- `MoneyFlowType.cs` enum
- `MoneyFlowAttachment.cs` record (optional)

### Step 2: Create Commands & Validators
- `CreateMoneyFlowCommand` with validator
- `UpdateMoneyFlowCommand` with validator
- `DeleteMoneyFlowCommand` with validator

### Step 3: Create Queries
- `GetMoneyFlowQuery` with handler
- `ListMoneyFlowsQuery` with filtering and pagination

### Step 4: Create DTOs
- `MoneyFlowDto` - full details
- `MoneyFlowListDto` - list view with less detail

### Step 5: Create EF Configurations
- `MoneyFlowConfiguration.cs` - table mapping, indexes, FK relationships
- `MoneyFlowAttachmentConfiguration.cs` (optional)

### Step 6: Update DbContext
Add DbSets to `PropertyManagementDbContext.cs`:
```csharp
public DbSet<MoneyFlow> MoneyFlows => Set<MoneyFlow>();
public DbSet<MoneyFlowAttachment> MoneyFlowAttachments => Set<MoneyFlowAttachment>(); // optional
```

### Step 7: Create Migration
```bash
dotnet ef migrations add Phase2B_MoneyFlows -o Infrastructure/Persistence/Migrations
```

### Step 8: Create Endpoints
- `MoneyFlowEndpoints.cs` - Map all routes

### Step 9: Register Endpoints
Add to `Program.cs`:
```csharp
app.MapMoneyFlowEndpoints();
```

### Step 10: Test Everything
Create test data and verify all operations work.

## Testing Requirements

### Test Data to Create

Using existing test data from Phase 2A:
- Property: "Sunset Villa" (id: `b66459e4-e65f-4904-80b9-0b3c7b676686`)
- Tenant: "John Smith" (id: `ef43349e-0387-44e0-9671-c86bb02cad64`)
- Lease: Active lease (id: `c51747b5-8e8a-46d1-9a90-db338a2f48b0`)
- Expense Categories: Council Tax, Maintenance, etc.

**Create MoneyFlows:**

1. **Income - Rental Payment** (£1,500 GBP)
   - Type: Income
   - Linked to: Property, Tenant, Lease
   - Income Source: "Rental Income"

2. **Income - Airbnb Booking** (£850 GBP)
   - Type: Income
   - Linked to: Property only
   - Income Source: "Airbnb Booking"

3. **Expense - Council Tax** (£250.50 GBP)
   - Type: Expense
   - Category: Council Tax
   - Linked to: Property

4. **Expense - Maintenance** (£450 GBP)
   - Type: Expense
   - Category: Maintenance & Repairs
   - Linked to: Property

5. **Expense - Utilities** (CHF 120.00)
   - Type: Expense
   - Currency: CHF (test multi-currency)
   - Category: Utilities
   - Linked to: Property

### Verification Checklist

- [ ] Build succeeds with no errors
- [ ] Migration created and applied successfully
- [ ] All 5+ endpoints working in Swagger
- [ ] Created 5+ money flows (mix of income/expense)
- [ ] Multi-currency working (GBP, CHF tested)
- [ ] Foreign key relationships enforced
- [ ] Validation rules working (amount > 0, valid currency, etc.)
- [ ] Filtering by property works
- [ ] Filtering by type (Income/Expense) works
- [ ] Filtering by date range works
- [ ] Filtering by category works
- [ ] Pagination works
- [ ] Search by description works
- [ ] Sorting by date (descending) works
- [ ] Can update money flow
- [ ] Can delete money flow
- [ ] No breaking changes to Phase 2A features

## Important Technical Notes

### Follow Phase 2A Patterns

1. **Use Vertical Slice Architecture** - Self-contained feature folder
2. **CQRS with MediatR** - Commands return DTOs, Queries return DTOs
3. **FluentValidation** - All commands and queries validated
4. **AutoMapper** - Map entities to DTOs (or manual mapping)
5. **Minimal APIs** - Use `RouteGroupBuilder` pattern
6. **OpenAPI Documentation** - Add summaries, produces, validates
7. **TransactionBehavior** - Automatic transactions (already configured)
8. **Structured Logging** - Use Serilog for all operations

### Database Configuration

**Connection String:** Uses `Host=postgres` (container network, not localhost)

**No Retry Strategy:** The DbContext does NOT use `EnableRetryOnFailure` because it conflicts with `TransactionBehavior`. This was fixed in Phase 2A.

### EF Core Configurations

**Example pattern from Phase 2A:**
```csharp
public class MoneyFlowConfiguration : IEntityTypeConfiguration<MoneyFlow>
{
    public void Configure(EntityTypeBuilder<MoneyFlow> builder)
    {
        builder.ToTable("money_flows", "property_management");

        builder.HasKey(mf => mf.Id);

        builder.Property(mf => mf.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(mf => mf.Currency)
            .HasMaxLength(3)
            .IsRequired();

        // Foreign keys
        builder.HasOne<Property>()
            .WithMany()
            .HasForeignKey(mf => mf.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(mf => new { mf.PropertyId, mf.Date })
            .HasDatabaseName("idx_money_flows_property_date");
    }
}
```

## Success Criteria

✅ MoneyFlows CRUD fully functional
✅ Foreign key relationships working (Property, ExpenseCategory, Tenant, Lease)
✅ Multi-currency support verified (GBP, CHF, EUR, USD)
✅ Advanced filtering implemented (property, type, date range, category, tenant)
✅ Pagination working
✅ Validation rules enforced
✅ Migration applied successfully
✅ 5+ test money flows created
✅ All endpoints documented in Swagger
✅ No breaking changes to Phase 2A features

## Deliverables

1. **Working MoneyFlows feature** - Full CRUD operations
2. **Migration** - `Phase2B_MoneyFlows` applied successfully
3. **Test data** - At least 5 money flows created (income + expense mix)
4. **PR documentation** - `PR_PHASE_2B.md` with testing results
5. **Pull Request** - Created and ready for review

## Current Environment

**Start Command:**
```bash
cd /workspace/money-flows/backend
git checkout main
git pull origin main
dotnet tool restore
dotnet build
dotnet ef database update  # Apply any pending migrations
cd src/Api
dotnet run
```

**API Access:**
- Base URL: http://localhost:5001
- Swagger: http://localhost:5001/swagger
- Health: http://localhost:5001/health

**Database:**
- Host: postgres:5432
- Database: property_mgmt
- Schema: property_management

Good luck! Follow the Phase 2A patterns and you'll have this done quickly. Focus on core CRUD first, file attachments can be Phase 3.
