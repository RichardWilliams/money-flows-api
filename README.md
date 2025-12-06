# UK Property Management API

A financial management system for tracking income and expenses across UK rental properties.

## Overview

This API powers a property management system designed for managing 3 UK rental properties from Switzerland. It provides comprehensive financial tracking, reporting capabilities, and will eventually integrate with platforms like Airbnb and Sykes Cottages.

## Tech Stack

- **.NET 10** - Latest .NET framework
- **PostgreSQL 16** - Database with dual schema support
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Request validation
- **AutoMapper** - Object mapping
- **Serilog** - Structured logging
- **EF Core** - ORM and migrations
- **xUnit** - Testing framework

## Architecture

This project follows **Vertical Slice Architecture** with CQRS pattern:

```
Features/
  {FeatureName}/
    Domain/              # Pure business logic, entities, interfaces
    Application/         # Commands, Queries, DTOs, Validators, Profiles
    Infrastructure/      # Repositories, Persistence, Mappers
    Endpoints/           # Minimal API endpoints
    Feature.cs           # Service + Endpoint registration
```

### Key Patterns

1. **CQRS with MediatR** - Commands for writes, Queries for reads
2. **FluentValidation** - Automatic validation via pipeline
3. **Repository Pattern + Unit of Work** - Clean data access
4. **Minimal APIs** - Lightweight endpoint definitions
5. **Domain Models as Records** - Immutable entities

## Getting Started

### Prerequisites

- Docker Desktop
- Visual Studio Code with Dev Containers extension
- Git

### Quick Start

1. Clone the repository:
```bash
git clone <repository-url>
cd money-flows/backend
```

2. Open in VS Code:
```bash
code .
```

3. When prompted, click "Reopen in Container" or press `F1` and select "Dev Containers: Reopen in Container"

4. The devcontainer will:
   - Install .NET 10
   - Start PostgreSQL 16
   - Restore dependencies
   - Build the project

5. Run the application:
```bash
dotnet run --project src/Api
```

6. Access the API:
   - Swagger UI: http://localhost:5000/swagger
   - Health checks: http://localhost:5000/health

### Manual Setup (Without Devcontainer)

1. Install .NET 10 SDK
2. Install PostgreSQL 16
3. Update connection string in `appsettings.Development.json`
4. Run migrations:
```bash
dotnet ef database update --project src/Api
```
5. Run the application:
```bash
dotnet run --project src/Api
```

## Database

### Dual Schema Approach

The application uses two PostgreSQL schemas:

- **`property_management`** - New application schema (default)
- **`home_accounts`** - Legacy schema (for existing data)

This approach ensures:
- Zero data loss during migration
- Gradual migration of legacy data
- Backward compatibility
- No downtime

### Migrations

Create a new migration:
```bash
dotnet ef migrations add <MigrationName> --project src/Api
```

Apply migrations:
```bash
dotnet ef database update --project src/Api
```

Rollback migration:
```bash
dotnet ef database update <PreviousMigrationName> --project src/Api
```

## Testing

Run all tests:
```bash
dotnet test
```

Run with coverage:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Project Structure

```
backend/
├── .devcontainer/           # Dev container configuration
├── .github/                 # GitHub Actions workflows
├── src/
│   ├── Api/                 # Main API project
│   │   ├── Features/        # Feature slices
│   │   ├── Infrastructure/  # Cross-cutting infrastructure
│   │   ├── Shared/          # Shared models, behaviors, exceptions
│   │   └── Program.cs       # Application entry point
│   └── Api.Tests/           # Test project
├── docs/                    # Documentation
├── PropertyManagement.sln   # Solution file
└── README.md
```

## Configuration

Key configuration sections in `appsettings.json`:

### Database
```json
"DatabaseSettings": {
  "ConnectionString": "Host=localhost;Database=property_mgmt;Username=postgres;Password=postgres"
}
```

### Attachments
```json
"AttachmentSettings": {
  "BasePath": "/app/attachments",
  "MaxFileSizeMb": 10,
  "AllowedExtensions": [".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx", ".xls", ".xlsx"]
}
```

### CORS
```json
"CorsSettings": {
  "AllowedOrigins": ["http://localhost:5173", "http://localhost:3000"]
}
```

## API Documentation

Once running, visit:
- Swagger UI: http://localhost:5000/swagger
- OpenAPI spec: http://localhost:5000/swagger/v1/swagger.json

## Health Checks

The API exposes three health check endpoints:

- `/health` - Overall health status
- `/health/ready` - Readiness probe (for k8s)
- `/health/live` - Liveness probe (for k8s)

## Logging

The application uses Serilog with structured logging:

- **Console**: Compact JSON format (for log aggregation)
- **File**: Rolling daily logs in `logs/` directory (30-day retention)

Logs include:
- Request/response logging
- Command/query execution times
- Validation failures
- Database transactions
- Errors and exceptions

## Contributing

1. Create a feature branch from `develop`
2. Follow the Vertical Slice Architecture pattern
3. Write tests for all new features
4. Ensure CI pipeline passes
5. Create a pull request to `develop`

## Backward Compatibility

**CRITICAL**: This API maintains backward compatibility as a core principle:

- No API versioning (no `/v1` prefix)
- Additive changes only
- Deprecation warnings via response headers
- 90-day sunset window for breaking changes
- All changes documented in CHANGELOG.md

## Further Reading

- [ARCHITECTURE.md](docs/ARCHITECTURE.md) - Detailed architecture documentation
- [SETUP.md](docs/SETUP.md) - Development environment setup guide
- [Implementation Prompt](../files/CLAUDE_CODE_IMPLEMENTATION_PROMPT.md) - Full implementation requirements

## License

Private - All rights reserved
