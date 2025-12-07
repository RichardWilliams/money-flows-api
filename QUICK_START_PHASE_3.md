# Quick Start Guide: Phase 3

## Starting a New Claude Code Session for Phase 3

### Copy This to Start Your New Chat:

```
I need to implement Phase 3 of the UK Property Management API.

Please read the implementation prompt at:
/workspace/money-flows/backend/PHASE_3_PROMPT.md

Start by understanding what was completed in Phase 2B by reviewing:
/workspace/money-flows/backend/PHASE_2B_SUMMARY.md

Then implement Phase 3 following the same patterns and architecture.
```

---

## What Phase 3 Includes

### Priority 1: Financial Reports (Phase 3A)
- **Property Financial Summary** - Income vs expenses for a single property
- **Portfolio Summary** - Aggregate view across all properties
- **Monthly Cash Flow Report** - Month-by-month breakdown

### Priority 2: Analytics Dashboard (Phase 3B)
- **Dashboard Overview** - KPIs and trends
- **Expense Breakdown** - Category analysis
- **Income Sources** - Source analysis

### Priority 3: File Attachments (Phase 3C)
- **Upload Attachments** - Add receipts/invoices to money flows
- **Download Attachments** - Retrieve uploaded files
- **Delete Attachments** - Remove files

---

## Current State

✅ **Phase 1**: Infrastructure complete
✅ **Phase 2A**: Properties, Expenses, Tenants, Leases complete
✅ **Phase 2B**: MoneyFlows complete (commit: 3a0f9e9)

**Database Schema**:
- `properties` - Property listings
- `tenants` - Tenant records
- `leases` - Lease agreements
- `expenses` - Individual expenses
- `expense_categories` - 13 UK expense categories
- `expense_attachments` - File attachments for expenses
- `money_flows` - Income and expense transactions ⭐ NEW

**API Running**: http://localhost:8080

---

## Files to Reference

When implementing Phase 3, reference these existing implementations:

### For Reports/Analytics (read-only queries):
- Check: `Features/Expenses/Application/Queries/ListExpensesQuery.cs`
- Pattern: No domain layer, just queries + DTOs + endpoints

### For File Attachments:
- Check: `Features/Expenses/Application/Commands/UploadAttachmentCommand.cs` (if exists)
- Check: `Infrastructure/Configuration/AttachmentSettings.cs`
- Storage: `/app/attachments/`

### For Endpoints:
- Check: `Features/MoneyFlows/Endpoints/MoneyFlowEndpoints.cs`
- Pattern: RouteGroupBuilder, minimal APIs

---

## Key Implementation Notes

1. **Reports are READ-ONLY**
   - No domain models needed
   - Just queries, DTOs, and endpoints
   - Use `.AsNoTracking()` for performance

2. **Follow CQRS**
   - Queries for reports/analytics
   - Commands for file operations
   - Use MediatR

3. **Multi-Currency**
   - Money flows support GBP, CHF, EUR, USD
   - Reports should handle currency conversion (optional)

4. **File Security**
   - Validate file types and sizes
   - Sanitize file names
   - Use streaming for downloads

---

## Testing Checklist

After implementing each feature set:

- [ ] Build succeeds
- [ ] Endpoints visible in Swagger
- [ ] Test with curl or Swagger UI
- [ ] Create test data
- [ ] Verify calculations/responses
- [ ] Commit changes

---

## Git Workflow

```bash
# Start from latest main
git checkout main
git pull origin main

# After implementation
git add .
git commit -m "feat: Implement Phase 3A - Financial Reports"
git push origin main

# Or create a branch if you prefer
git checkout -b feature/phase-3
# ... implement ...
git push origin feature/phase-3
# Then create PR
```

---

## Questions to Ask Claude

If you get stuck, try asking:

- "Show me an example of a read-only query from Phase 2"
- "How should I structure the reports feature folder?"
- "What DTOs do I need for the property summary report?"
- "How do I handle file uploads in .NET 8 Minimal APIs?"
- "Show me the existing expense attachment implementation"

---

## Success Criteria

You'll know Phase 3 is complete when:

✅ All reports return accurate calculations
✅ Dashboard shows correct KPIs
✅ Can upload files to money flows
✅ Can download/delete attachments
✅ All endpoints documented in Swagger
✅ Test data demonstrates all features

---

## Repository

**GitHub**: https://github.com/RichardWilliams/money-flows-api
**Main Branch**: All changes pushed directly
**Latest Commit**: 093ba58 (Phase 3 prompt added)

---

Good luck with Phase 3! 🚀
