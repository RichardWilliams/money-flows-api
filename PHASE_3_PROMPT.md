# Phase 3: Advanced Features - Reports, Analytics & File Attachments

## Context

You are continuing development of the UK Property Management API. This is **Phase 3** - adding advanced features for reporting, analytics, and file attachments.

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

**Phase 2A:** Domain Features ✅
- **Properties** - UK properties with postcode validation
- **Expenses** - 13 UK expense categories, multi-currency
- **Tenants** - UK phone validation, emergency contacts
- **Leases** - Business rules, rent day validation, status management

**Phase 2B:** MoneyFlows ✅
- **Income & Expense Tracking** - Complete transaction system
- **Multi-currency** - GBP, CHF, EUR, USD
- **Advanced Filtering** - By property, type, date range, category, tenant
- **Relationships** - Links to properties, categories, tenants, leases
- **5+ Endpoints** - Full CRUD operations

**Database:**
- Host: `postgres` (container network)
- Database: `property_mgmt`
- Schema: `property_management`
- 8 tables exist: properties, expenses, expense_categories, expense_attachments, tenants, leases, money_flows, __EFMigrationsHistory

**Tools Available:**
- `dotnet tool restore` - Installs dotnet-ef 8.0.11
- `dotnet ef` - EF Core CLI tools

## Phase 3 Goal

Implement **three major feature sets** to provide business intelligence and document management capabilities:

1. **Financial Reports** - Income vs Expense summaries, property performance
2. **Analytics Dashboard** - Key metrics, trends, visualizations
3. **File Attachments** - Document management for money flows (receipts, invoices)

---

## Feature Set 1: Financial Reports

### Requirements

Implement comprehensive financial reporting endpoints that provide business insights.

#### 1.1 Property Financial Summary

**Endpoint:** `GET /api/reports/property-summary/{propertyId}`

**Query Parameters:**
- `dateFrom` (optional) - Start date for report period
- `dateTo` (optional) - End date for report period
- `currency` (optional) - Convert all amounts to this currency (default: GBP)

**Response:**
```json
{
  "propertyId": "guid",
  "propertyName": "Sunset Villa",
  "reportPeriod": {
    "from": "2024-01-01",
    "to": "2024-12-31"
  },
  "currency": "GBP",
  "income": {
    "totalAmount": 18000.00,
    "transactionCount": 12,
    "breakdown": [
      {
        "source": "Rental Income",
        "amount": 15000.00,
        "count": 10
      },
      {
        "source": "Airbnb Booking",
        "amount": 3000.00,
        "count": 2
      }
    ]
  },
  "expenses": {
    "totalAmount": 5250.50,
    "transactionCount": 15,
    "breakdown": [
      {
        "categoryId": "guid",
        "categoryName": "Council Tax",
        "amount": 3000.00,
        "count": 12
      },
      {
        "categoryId": "guid",
        "categoryName": "Maintenance & Repairs",
        "amount": 2250.50,
        "count": 3
      }
    ]
  },
  "netIncome": 12749.50,
  "profitMargin": 70.86
}
```

#### 1.2 Portfolio Summary

**Endpoint:** `GET /api/reports/portfolio-summary`

**Query Parameters:**
- `dateFrom` (optional) - Start date
- `dateTo` (optional) - End date
- `currency` (optional) - Default: GBP

**Response:**
```json
{
  "reportPeriod": {
    "from": "2024-01-01",
    "to": "2024-12-31"
  },
  "currency": "GBP",
  "totalProperties": 5,
  "totalIncome": 90000.00,
  "totalExpenses": 32500.00,
  "netIncome": 57500.00,
  "averageROI": 12.5,
  "properties": [
    {
      "propertyId": "guid",
      "propertyName": "Sunset Villa",
      "income": 18000.00,
      "expenses": 5250.50,
      "netIncome": 12749.50,
      "occupancyRate": 100.0
    }
  ],
  "topExpenseCategories": [
    {
      "categoryName": "Council Tax",
      "totalAmount": 15000.00,
      "percentage": 46.15
    }
  ]
}
```

#### 1.3 Monthly Cash Flow Report

**Endpoint:** `GET /api/reports/cash-flow`

**Query Parameters:**
- `propertyId` (optional) - Filter by property
- `year` (required) - Year for report
- `currency` (optional) - Default: GBP

**Response:**
```json
{
  "year": 2024,
  "currency": "GBP",
  "months": [
    {
      "month": 1,
      "monthName": "January",
      "income": 7500.00,
      "expenses": 2100.00,
      "netCashFlow": 5400.00,
      "cumulativeCashFlow": 5400.00
    },
    {
      "month": 2,
      "monthName": "February",
      "income": 7500.00,
      "expenses": 2300.00,
      "netCashFlow": 5200.00,
      "cumulativeCashFlow": 10600.00
    }
  ],
  "yearTotal": {
    "income": 90000.00,
    "expenses": 32500.00,
    "netCashFlow": 57500.00
  }
}
```

### Implementation Structure

```
Features/Reports/
├── Application/
│   ├── Queries/
│   │   ├── GetPropertySummaryQuery.cs
│   │   ├── GetPortfolioSummaryQuery.cs
│   │   └── GetCashFlowReportQuery.cs
│   └── Dtos/
│       ├── PropertySummaryDto.cs
│       ├── PortfolioSummaryDto.cs
│       ├── CashFlowReportDto.cs
│       └── ... (supporting DTOs)
└── Endpoints/
    └── ReportEndpoints.cs
```

**No Domain Layer needed** - Reports are read-only queries over existing data.

---

## Feature Set 2: Analytics Dashboard

### Requirements

Provide key performance indicators and metrics for dashboard visualization.

#### 2.1 Dashboard Overview

**Endpoint:** `GET /api/analytics/dashboard`

**Query Parameters:**
- `period` (optional) - "month", "quarter", "year", "all" (default: "month")

**Response:**
```json
{
  "period": "month",
  "periodStart": "2024-12-01",
  "periodEnd": "2024-12-31",
  "kpis": {
    "totalIncome": 7500.00,
    "totalExpenses": 2100.00,
    "netIncome": 5400.00,
    "profitMargin": 72.0,
    "activeProperties": 5,
    "activeTenants": 8,
    "occupancyRate": 95.0,
    "averageRentPerProperty": 1500.00
  },
  "trends": {
    "incomeChange": {
      "amount": 500.00,
      "percentage": 7.14,
      "direction": "up"
    },
    "expenseChange": {
      "amount": -200.00,
      "percentage": -8.70,
      "direction": "down"
    }
  },
  "upcomingLeaseExpirations": [
    {
      "leaseId": "guid",
      "propertyName": "Sunset Villa",
      "tenantName": "John Smith",
      "expirationDate": "2025-05-31",
      "daysUntilExpiration": 152
    }
  ],
  "recentTransactions": [
    {
      "id": "guid",
      "date": "2024-12-01",
      "type": "Income",
      "amount": 1500.00,
      "description": "Monthly rental payment"
    }
  ]
}
```

#### 2.2 Expense Breakdown

**Endpoint:** `GET /api/analytics/expense-breakdown`

**Query Parameters:**
- `propertyId` (optional)
- `dateFrom` (optional)
- `dateTo` (optional)

**Response:**
```json
{
  "totalExpenses": 32500.00,
  "categories": [
    {
      "categoryId": "guid",
      "categoryName": "Council Tax",
      "amount": 15000.00,
      "percentage": 46.15,
      "transactionCount": 12,
      "averagePerTransaction": 1250.00
    }
  ],
  "topProperties": [
    {
      "propertyId": "guid",
      "propertyName": "Property A",
      "totalExpenses": 8500.00,
      "percentage": 26.15
    }
  ]
}
```

#### 2.3 Income Sources

**Endpoint:** `GET /api/analytics/income-sources`

**Query Parameters:**
- `propertyId` (optional)
- `dateFrom` (optional)
- `dateTo` (optional)

**Response:**
```json
{
  "totalIncome": 90000.00,
  "sources": [
    {
      "source": "Rental Income",
      "amount": 75000.00,
      "percentage": 83.33,
      "transactionCount": 60
    },
    {
      "source": "Airbnb Booking",
      "amount": 15000.00,
      "percentage": 16.67,
      "transactionCount": 12
    }
  ],
  "byProperty": [
    {
      "propertyId": "guid",
      "propertyName": "Sunset Villa",
      "income": 18000.00,
      "percentage": 20.0
    }
  ]
}
```

### Implementation Structure

```
Features/Analytics/
├── Application/
│   ├── Queries/
│   │   ├── GetDashboardQuery.cs
│   │   ├── GetExpenseBreakdownQuery.cs
│   │   └── GetIncomeSourcesQuery.cs
│   └── Dtos/
│       ├── DashboardDto.cs
│       ├── ExpenseBreakdownDto.cs
│       ├── IncomeSourcesDto.cs
│       └── ... (supporting DTOs)
└── Endpoints/
    └── AnalyticsEndpoints.cs
```

---

## Feature Set 3: File Attachments for MoneyFlows

### Requirements

Implement document management for money flows (receipts, invoices, etc.).

#### 3.1 Domain Model

**Update existing MoneyFlowAttachment:**

```csharp
namespace PropertyManagement.Api.Features.MoneyFlows.Domain;

public record MoneyFlowAttachment
{
    public Guid Id { get; init; }
    public Guid MoneyFlowId { get; init; }
    public required string FileName { get; init; }
    public required string OriginalFileName { get; init; }
    public required string StoragePath { get; init; }
    public long FileSizeBytes { get; init; }
    public required string ContentType { get; init; }
    public DateTime UploadedAt { get; init; }
    public string? Description { get; init; }
}
```

#### 3.2 Storage Structure

```
/app/attachments/moneyflows/
  {year}/
    {month}/
      {guid}-{sanitized-filename}
```

Example: `/app/attachments/moneyflows/2024/12/a1b2c3d4-e5f6-invoice.pdf`

#### 3.3 API Endpoints

**Upload Attachment:**
- `POST /api/moneyflows/{id}/attachments`
- **Content-Type:** `multipart/form-data`
- **Request:**
  ```
  file: [binary]
  description: "December rent receipt" (optional)
  ```
- **Response:**
  ```json
  {
    "id": "guid",
    "moneyFlowId": "guid",
    "fileName": "a1b2c3d4-e5f6-invoice.pdf",
    "originalFileName": "invoice.pdf",
    "fileSizeBytes": 245678,
    "contentType": "application/pdf",
    "uploadedAt": "2024-12-07T10:30:00Z",
    "description": "December rent receipt"
  }
  ```

**List Attachments:**
- `GET /api/moneyflows/{id}/attachments`
- **Response:**
  ```json
  {
    "moneyFlowId": "guid",
    "attachments": [
      {
        "id": "guid",
        "fileName": "a1b2c3d4-e5f6-invoice.pdf",
        "originalFileName": "invoice.pdf",
        "fileSizeBytes": 245678,
        "contentType": "application/pdf",
        "uploadedAt": "2024-12-07T10:30:00Z",
        "downloadUrl": "/api/moneyflows/{id}/attachments/{attachmentId}/download"
      }
    ]
  }
  ```

**Download Attachment:**
- `GET /api/moneyflows/{id}/attachments/{attachmentId}/download`
- **Response:** Binary file stream with proper Content-Type header

**Delete Attachment:**
- `DELETE /api/moneyflows/{id}/attachments/{attachmentId}`
- **Response:** 204 No Content

#### 3.4 Validation Rules

- File size: Max 10MB (from existing AttachmentSettings)
- Allowed extensions: .pdf, .jpg, .jpeg, .png, .doc, .docx, .xls, .xlsx
- MoneyFlow must exist
- User must have permission to upload to that money flow
- Prevent duplicate uploads (same file hash)

#### 3.5 Database Schema

**Table: money_flow_attachments** (already defined in Phase 2B, now implement)

```sql
CREATE TABLE property_management.money_flow_attachments (
    id UUID PRIMARY KEY,
    money_flow_id UUID NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    original_file_name VARCHAR(255) NOT NULL,
    storage_path VARCHAR(500) NOT NULL,
    file_size_bytes BIGINT NOT NULL,
    content_type VARCHAR(100) NOT NULL,
    uploaded_at TIMESTAMP NOT NULL,
    description VARCHAR(500) NULL,

    CONSTRAINT fk_money_flow_attachments_money_flow
        FOREIGN KEY (money_flow_id)
        REFERENCES property_management.money_flows(id)
        ON DELETE CASCADE
);

CREATE INDEX idx_money_flow_attachments_money_flow
    ON property_management.money_flow_attachments(money_flow_id);
```

#### 3.6 Implementation Structure

```
Features/MoneyFlows/
├── Application/
│   ├── Commands/
│   │   ├── UploadAttachmentCommand.cs
│   │   └── DeleteAttachmentCommand.cs
│   ├── Queries/
│   │   ├── ListAttachmentsQuery.cs
│   │   └── DownloadAttachmentQuery.cs
│   └── Services/
│       └── FileStorageService.cs (handles physical file operations)
```

**Update MoneyFlowEndpoints.cs** to add attachment routes.

---

## Implementation Priority

### Phase 3A: Financial Reports (Highest Value)
1. Property Financial Summary
2. Portfolio Summary
3. Monthly Cash Flow Report

### Phase 3B: Analytics Dashboard
1. Dashboard Overview
2. Expense Breakdown
3. Income Sources

### Phase 3C: File Attachments
1. Upload Attachment
2. List Attachments
3. Download Attachment
4. Delete Attachment

---

## Technical Requirements

### Follow Existing Patterns

1. **Vertical Slice Architecture** - Self-contained feature folders
2. **CQRS with MediatR** - Queries for reports, Commands for attachments
3. **FluentValidation** - All queries and commands validated
4. **Minimal APIs** - Clean endpoint definitions
5. **OpenAPI Documentation** - Swagger summaries for all endpoints
6. **Structured Logging** - Serilog for all operations

### Performance Considerations

**Reports & Analytics:**
- Use AsNoTracking() for all read operations
- Consider caching frequently requested reports (optional)
- Optimize queries with proper indexes
- Use projections to minimize data transfer

**File Attachments:**
- Stream large files (don't load into memory)
- Use async I/O operations
- Validate file types before processing
- Sanitize file names to prevent directory traversal attacks

### Error Handling

- Return 404 if money flow doesn't exist
- Return 400 for invalid file types/sizes
- Return 413 for files exceeding size limit
- Return 500 for file system errors (with logging)

---

## Testing Requirements

### For Each Feature Set:

1. **Build Verification**
   - `dotnet build` succeeds
   - No compiler warnings

2. **Migration Applied**
   - For attachments: Create and apply migration
   - Verify tables created

3. **Endpoint Testing**
   - Test via Swagger or curl
   - Verify response formats match specifications
   - Test error cases

4. **Integration Testing**
   - Reports: Create test data, verify calculations
   - Analytics: Verify KPIs are accurate
   - Attachments: Upload, download, delete files

### Test Data Scenarios

**For Reports:**
- Multiple properties with varying income/expense patterns
- Different date ranges
- Multi-currency transactions

**For Analytics:**
- Mix of income sources
- Various expense categories
- Active and expired leases

**For Attachments:**
- PDF invoices
- Image receipts
- Different file sizes
- Edge cases (max size, invalid types)

---

## Success Criteria

### Phase 3A: Financial Reports
- [ ] Property summary endpoint returns accurate calculations
- [ ] Portfolio summary aggregates all properties correctly
- [ ] Cash flow report shows monthly breakdown
- [ ] All date range filters work
- [ ] Multi-currency conversion works (if implemented)

### Phase 3B: Analytics Dashboard
- [ ] Dashboard shows accurate KPIs
- [ ] Expense breakdown percentages sum to 100%
- [ ] Income sources correctly categorized
- [ ] Trend calculations accurate
- [ ] All filtering options work

### Phase 3C: File Attachments
- [ ] Can upload files via API
- [ ] Files stored in correct directory structure
- [ ] Can list all attachments for a money flow
- [ ] Can download attachments
- [ ] Can delete attachments (file removed from disk)
- [ ] File size validation enforced
- [ ] File type validation enforced
- [ ] Orphaned files cleaned up when money flow deleted

---

## Current Environment

**Start Command:**
```bash
cd /workspace/money-flows/backend
git checkout main
git pull origin main
dotnet tool restore
dotnet build
cd src/Api
dotnet run
```

**API Access:**
- Base URL: http://localhost:8080
- Swagger: http://localhost:8080/swagger
- Health: http://localhost:8080/health

**Database:**
- Host: postgres:5432
- Database: property_mgmt
- Schema: property_management

**Existing Endpoints:**
- `/api/properties` - CRUD for properties
- `/api/expenses` - CRUD for expenses
- `/api/expense-categories` - List categories
- `/api/tenants` - CRUD for tenants
- `/api/leases` - CRUD for leases
- `/api/moneyflows` - CRUD for money flows with filtering

---

## Deliverables

1. **Working Features** - All three feature sets functional
2. **Migration** - For attachments table (if not already exists)
3. **Test Data** - Sufficient data to demonstrate all reports/analytics
4. **Documentation** - Summary of implementation and testing results
5. **Git Commit** - Clean commit with descriptive message

---

## Optional Enhancements (Bonus)

If time permits:

1. **Export Reports** - Add `format=pdf` or `format=excel` query parameter
2. **Scheduled Reports** - Email reports on schedule (future feature)
3. **Charts Data** - Return data formatted for charting libraries
4. **File Thumbnails** - Generate thumbnails for images
5. **Bulk Upload** - Upload multiple files at once
6. **Attachment Tags** - Categorize attachments (receipt, invoice, contract)

---

## Notes

- Focus on **Phase 3A (Reports)** first - highest business value
- Phase 3B and 3C can be done in parallel if time permits
- File attachments are lower priority than reports
- All features should follow existing code patterns from Phase 2A and 2B
- Use existing infrastructure (AttachmentSettings, logging, etc.)

Good luck! This is an ambitious phase, so prioritize the financial reports first. The analytics and attachments can come later if needed.
