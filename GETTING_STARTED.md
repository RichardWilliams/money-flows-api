# Getting Started - Money Flows API Development

## Quick Reference

### Repository
- **GitHub:** https://github.com/RichardWilliams/money-flows-api
- **Current Branch:** `main`
- **Location:** `/workspace/money-flows/backend`

### What's Completed

- ✅ **Phase 1:** Infrastructure, CQRS, Logging, Health Checks
- ✅ **Phase 2A:** Properties, Expenses (13 UK categories), Tenants, Leases
- ⏸️ **Phase 2B:** MoneyFlows feature (IN PROGRESS - see PHASE_2B_MONEYFLOWS_PROMPT.md)

### Starting a New Phase

1. **Pull latest code:**
   ```bash
   cd /workspace/money-flows/backend
   git checkout main
   git pull origin main
   ```

2. **Restore tools:**
   ```bash
   dotnet tool restore  # Installs dotnet-ef 8.0.11
   ```

3. **Build project:**
   ```bash
   dotnet build
   ```

4. **Start API:**
   ```bash
   cd src/Api
   dotnet run
   ```

5. **Access:**
   - API: http://localhost:5001
   - Swagger: http://localhost:5001/swagger
   - Health: http://localhost:5001/health

### Database

- **Host:** `postgres:5432` (container network)
- **Database:** `property_mgmt`
- **Schema:** `property_management`
- **Connection String:** `Host=postgres;Database=property_mgmt;Username=postgres;Password=postgres`

### Creating Migrations

```bash
dotnet ef migrations add <MigrationName> -o Infrastructure/Persistence/Migrations
dotnet ef database update
```

### Phase Implementation Prompts

Each phase has a detailed prompt with requirements and test plan:

- **Phase 2B (Current):** `PHASE_2B_MONEYFLOWS_PROMPT.md` - MoneyFlows feature
- **Phase 3 (Next):** TBD - Participants feature or advanced filtering

### Architecture Patterns (From Phase 2A)

**Follow these patterns:**
1. Vertical Slice Architecture (self-contained features)
2. CQRS with MediatR
3. FluentValidation for all commands/queries
4. Minimal APIs with OpenAPI documentation
5. TransactionBehavior for automatic transactions
6. NO retry strategy in DbContext (conflicts with TransactionBehavior)

### Existing Features (Phase 2A)

**Properties:**
- UK postcode validation (normalized to uppercase, no spaces)
- Property types: Detached, Semi-Detached, Terraced, Flat, etc.
- Status: Active, Archived, Under Maintenance

**Expenses:**
- 13 UK expense categories (Council Tax, Maintenance, Insurance, etc.)
- Multi-currency: GBP, CHF, EUR, USD
- Attachment metadata support

**Tenants:**
- UK phone validation (+44 7XXX XXX XXX, 07XXX XXX XXX)
- Emergency contacts
- Full name generation

**Leases:**
- Business rules (start < end date, rent day 1-28)
- Status: Active, Expired, Terminated
- Multi-currency rent tracking

### Test Data Available

From Phase 2A testing:
- Property: "Sunset Villa" (Detached, 4 bed, £450k)
- Expense Category: Council Tax
- Tenant: John Smith
- Lease: Active 12-month lease (£1,500/month)

### Common Commands

```bash
# Build
dotnet build

# Run API
cd src/Api && dotnet run

# Create migration
dotnet ef migrations add MigrationName -o Infrastructure/Persistence/Migrations

# Apply migrations
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove

# Git workflow
git checkout -b feature/phase-X-description
# ... make changes ...
git add .
git commit -m "feat: description"
git push origin feature/phase-X-description
# Create PR on GitHub
```

### Important Notes

1. **Connection String:** Uses `Host=postgres` (container network, not `localhost`)
2. **No Retry Strategy:** Removed from DbContext (conflicts with TransactionBehavior)
3. **Tools:** `dotnet-ef` available via `dotnet tool restore`
4. **Schema:** All tables in `property_management` schema
5. **Primary Keys:** All use GUIDs
6. **Audit Fields:** CreatedAt, UpdatedAt on all entities

### Creating a Pull Request

1. Push your branch
2. Create PR on GitHub
3. PR body should include:
   - Summary of features
   - Technical implementation details
   - Database changes
   - Test results
   - API endpoints added

See `PR_PHASE_2.md` for an example.

### Support Files

- **Implementation Status:** `/workspace/money-flows/IMPLEMENTATION_STATUS.md`
- **Original Requirements:** `/workspace/money-flows/files/CLAUDE_CODE_IMPLEMENTATION_PROMPT.md`
- **Phase 2A PR:** `/workspace/money-flows/backend/PR_PHASE_2.md`
- **Phase 2B Prompt:** `/workspace/money-flows/backend/PHASE_2B_MONEYFLOWS_PROMPT.md`
