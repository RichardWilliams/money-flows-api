# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Phase 1: Foundation & Infrastructure (Completed)

#### Added
- Initial project structure with .NET 10
- Vertical Slice Architecture foundation
- Devcontainer configuration for consistent development environment
- PostgreSQL 16 database with dual schema support (`property_management` and `home_accounts`)
- MediatR CQRS pattern implementation
- FluentValidation pipeline behavior for automatic request validation
- Serilog structured logging with JSON format for k8s compatibility
- Entity Framework Core 10 with Npgsql provider
- AutoMapper for DTO mapping
- Health check endpoints (`/health`, `/health/ready`, `/health/live`)
- Global exception handling with RFC 7231 problem details
- CORS configuration for frontend integration
- GitHub Actions CI/CD pipeline
- Comprehensive documentation (README, ARCHITECTURE, SETUP, API)
- Unit of Work pattern for transaction management
- Transaction pipeline behavior for automatic transaction wrapping
- Logging pipeline behavior for request/response tracking
- Shared models (PagedList, DatabaseSettings, AttachmentSettings, CorsSettings)
- Exception types (NotFoundException, DomainException)
- EditorConfig for consistent code formatting
- .gitignore for .NET projects
- Test project with xUnit, FluentAssertions, and Moq
- Example PagedList tests

#### Technical Details
- Framework: .NET 10
- Database: PostgreSQL 16
- ORM: Entity Framework Core 10
- Architecture: Vertical Slice + CQRS
- Logging: Serilog with JSON formatting
- Validation: FluentValidation
- Mapping: AutoMapper
- Testing: xUnit, FluentAssertions, Moq
- Health Checks: AspNetCore.HealthChecks.NpgSql
- API Documentation: Swagger/OpenAPI

## [Unreleased - Planned]

### Phase 2: Core MoneyFlows Feature
- MoneyFlows CRUD operations
- File attachment handling with date-first organization
- MoneyFlow domain model as immutable record
- MoneyFlow repository with filtering and pagination
- Validation for money flow operations
- AutoMapper profiles for MoneyFlow DTOs
- Integration tests for MoneyFlow endpoints
- Swagger documentation for MoneyFlow endpoints

### Phase 3: Properties & Participants Features
- Properties CRUD operations (replacing "Contexts")
- Property image upload
- Participants CRUD operations
- Participant types management
- Repository implementations
- Validation rules
- Integration tests

### Phase 4: Search, Filter, Export
- Advanced filtering for all entities
- Full-text search capabilities
- Export to CSV, Excel, PDF
- Custom date range reports
- Financial summary reports

### Phase 5: Pots Management System
- Virtual pots for expense allocation
- Pot rules and automation
- Pot balance tracking
- Pot transaction history

### Phase 6: Reminders & Recurring Services
- Recurring service definitions
- Reminder notifications
- Service due date tracking
- Email/push notifications

### Phase 7: User Roles & Authentication
- JWT authentication
- Role-based authorization
- User management
- Audit logging with user tracking

### Phase 8: Platform Integrations
- Airbnb API integration
- Sykes Cottages integration
- Automatic transaction import
- Data synchronization
- Webhook handling

## Version History

### [1.0.0-alpha] - 2025-01-15

#### Added
- Phase 1: Foundation & Infrastructure
- Initial release with basic infrastructure
- Health checks and monitoring
- Development environment setup
- Comprehensive documentation

---

## Migration Notes

### Backward Compatibility

This project follows a **no-breaking-changes** policy:

- No URL versioning (no `/v1` prefix)
- Additive changes only
- Deprecation warnings for 90 days
- Migration guides for breaking changes

### Breaking Changes Policy

If a breaking change is absolutely necessary:

1. Announce 90 days in advance
2. Add `X-Deprecated-Warning` header to affected endpoints
3. Update documentation with migration guide
4. Provide automated migration tools if possible
5. Monitor usage of deprecated features
6. Remove only after 90-day grace period

### Database Migrations

All database changes are managed through EF Core migrations:

```bash
# Create migration
dotnet ef migrations add MigrationName --project src/Api

# Apply migration
dotnet ef database update --project src/Api

# Rollback migration
dotnet ef database update PreviousMigrationName --project src/Api
```

---

## Support

For questions or issues:
- GitHub Issues: [Create an issue]
- Documentation: See `docs/` folder
- Email: support@example.com

---

## Contributors

- Development Team
- Built with Claude Code (Anthropic)

---

## License

Private - All Rights Reserved
