# Development Environment Setup

This guide will help you set up your development environment for the UK Property Management API.

## Prerequisites

### Required

- **Docker Desktop** - For running devcontainer
  - Download: https://www.docker.com/products/docker-desktop
  - Minimum version: 4.0+

- **Visual Studio Code** - Primary IDE
  - Download: https://code.visualstudio.com/
  - Minimum version: 1.80+

- **Git** - Version control
  - Download: https://git-scm.com/
  - Minimum version: 2.30+

### Recommended VS Code Extensions

The devcontainer will automatically install these, but for local development:

- **C# Dev Kit** (ms-dotnettools.csdevkit)
- **C#** (ms-dotnettools.csharp)
- **EditorConfig** (editorconfig.editorconfig)
- **REST Client** (humao.rest-client)
- **GitHub Copilot** (github.copilot) - Optional but recommended

## Setup Options

### Option 1: Devcontainer (Recommended)

This is the easiest and most consistent way to set up your environment.

1. **Install Docker Desktop**
   ```bash
   # Start Docker Desktop and ensure it's running
   docker --version
   ```

2. **Install VS Code Dev Containers Extension**
   - Open VS Code
   - Press `Ctrl+Shift+X` (or `Cmd+Shift+X` on Mac)
   - Search for "Dev Containers"
   - Install "Dev Containers" by Microsoft

3. **Clone and Open the Project**
   ```bash
   git clone <repository-url>
   cd money-flows/backend
   code .
   ```

4. **Reopen in Container**
   - When prompted, click "Reopen in Container"
   - Or press `F1` and select "Dev Containers: Reopen in Container"
   - Wait for the container to build (first time takes 5-10 minutes)

5. **Verify Setup**
   ```bash
   # Check .NET installation
   dotnet --version  # Should show 10.0.x

   # Check PostgreSQL connection
   psql -h db -U postgres -d property_mgmt  # Password: postgres

   # Run the application
   dotnet run --project src/Api
   ```

6. **Access the Application**
   - Swagger UI: http://localhost:5000/swagger
   - Health Check: http://localhost:5000/health

### Option 2: Local Development (Without Devcontainer)

If you prefer to run everything locally:

1. **Install .NET 10 SDK**
   ```bash
   # Download from https://dotnet.microsoft.com/download/dotnet/10.0
   dotnet --version  # Should show 10.0.x
   ```

2. **Install PostgreSQL 16**
   ```bash
   # macOS (using Homebrew)
   brew install postgresql@16
   brew services start postgresql@16

   # Windows (using installer)
   # Download from https://www.postgresql.org/download/windows/

   # Linux (Ubuntu/Debian)
   sudo apt-get install postgresql-16
   sudo systemctl start postgresql
   ```

3. **Create Database**
   ```bash
   # Connect to PostgreSQL
   psql -U postgres

   # Create database and user
   CREATE DATABASE property_mgmt;
   CREATE USER postgres WITH PASSWORD 'postgres';
   GRANT ALL PRIVILEGES ON DATABASE property_mgmt TO postgres;
   \q
   ```

4. **Clone Project**
   ```bash
   git clone <repository-url>
   cd money-flows/backend
   ```

5. **Update Configuration**
   - Edit `src/Api/appsettings.Development.json`
   - Update connection string if needed:
   ```json
   {
     "DatabaseSettings": {
       "ConnectionString": "Host=localhost;Database=property_mgmt;Username=postgres;Password=postgres"
     }
   }
   ```

6. **Install EF Core Tools**
   ```bash
   dotnet tool install --global dotnet-ef
   ```

7. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

8. **Apply Migrations**
   ```bash
   dotnet ef database update --project src/Api
   ```

9. **Run the Application**
   ```bash
   dotnet run --project src/Api
   ```

## Common Development Tasks

### Running the Application

```bash
# Development mode with hot reload
dotnet watch run --project src/Api

# Production mode
dotnet run --project src/Api --configuration Release
```

### Database Migrations

```bash
# Create a new migration
dotnet ef migrations add <MigrationName> --project src/Api

# Apply migrations
dotnet ef database update --project src/Api

# Rollback to specific migration
dotnet ef database update <PreviousMigrationName> --project src/Api

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/Api

# Generate SQL script
dotnet ef migrations script --project src/Api --output migration.sql
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "FullyQualifiedName~CreateMoneyFlowCommandHandlerTests"

# Run tests in watch mode
dotnet watch test
```

### Building

```bash
# Debug build
dotnet build

# Release build
dotnet build --configuration Release

# Clean build
dotnet clean
dotnet build
```

### Code Formatting

```bash
# Format code according to .editorconfig
dotnet format

# Check formatting without applying
dotnet format --verify-no-changes
```

## Troubleshooting

### Devcontainer Issues

**Problem: "Failed to connect to Docker"**
```bash
# Solution: Ensure Docker Desktop is running
docker ps  # Should list running containers
```

**Problem: "Port 5432 already in use"**
```bash
# Solution: Stop local PostgreSQL or change port in docker-compose.yml
sudo lsof -i :5432  # Find process using port
sudo kill -9 <PID>  # Kill the process
```

**Problem: "Devcontainer build fails"**
```bash
# Solution: Rebuild without cache
# In VS Code: F1 → "Dev Containers: Rebuild Container Without Cache"
```

### Database Issues

**Problem: "Connection refused to database"**
```bash
# Check if PostgreSQL is running
docker ps  # In devcontainer
# OR
brew services list  # On macOS
sudo systemctl status postgresql  # On Linux

# Check connection string in appsettings.Development.json
```

**Problem: "Migration failed"**
```bash
# Reset database
dotnet ef database drop --project src/Api --force
dotnet ef database update --project src/Api
```

### Build Issues

**Problem: "Package restore failed"**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore again
dotnet restore
```

**Problem: "CS0246: The type or namespace name 'X' could not be found"**
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

## Environment Variables

### Required

- `ASPNETCORE_ENVIRONMENT` - Development/Staging/Production
- `ConnectionStrings__Database` - PostgreSQL connection string

### Optional

- `Serilog__MinimumLevel__Default` - Logging level (Debug/Information/Warning/Error)
- `AttachmentSettings__BasePath` - File storage location
- `CorsSettings__AllowedOrigins` - Comma-separated origins

### Setting Environment Variables

**Devcontainer (docker-compose.yml):**
```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ConnectionStrings__Database=Host=db;Database=property_mgmt;Username=postgres;Password=postgres
```

**Local Development (.NET User Secrets):**
```bash
# Initialize user secrets
dotnet user-secrets init --project src/Api

# Set a secret
dotnet user-secrets set "ConnectionStrings:Database" "Host=localhost;Database=property_mgmt;Username=postgres;Password=postgres" --project src/Api

# List secrets
dotnet user-secrets list --project src/Api
```

**Environment File (.env):**
```bash
# Create .env file (not committed)
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__Database=Host=localhost;Database=property_mgmt;Username=postgres;Password=postgres

# Load in terminal (Linux/macOS)
export $(cat .env | xargs)

# Load in PowerShell (Windows)
Get-Content .env | ForEach-Object {
    $name, $value = $_.split('=')
    Set-Content env:\$name $value
}
```

## IDE Configuration

### VS Code Settings

Recommended settings (`.vscode/settings.json`):

```json
{
  "omnisharp.enableRoslynAnalyzers": true,
  "omnisharp.enableEditorConfigSupport": true,
  "editor.formatOnSave": true,
  "editor.codeActionsOnSave": {
    "source.fixAll": true
  },
  "dotnet.defaultSolution": "PropertyManagement.sln"
}
```

### VS Code Launch Configuration

Debug configuration (`.vscode/launch.json`):

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/src/Api/bin/Debug/net10.0/PropertyManagement.Api.dll",
      "args": [],
      "cwd": "${workspaceFolder}/src/Api",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      },
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  ]
}
```

## Next Steps

Once your environment is set up:

1. Read [ARCHITECTURE.md](ARCHITECTURE.md) to understand the codebase structure
2. Review existing features in `src/Api/Features/`
3. Check out the [implementation prompt](../files/CLAUDE_CODE_IMPLEMENTATION_PROMPT.md) for development guidelines
4. Start building features following the Vertical Slice Architecture pattern

## Getting Help

- **Issues**: Create an issue in the repository
- **Questions**: Check the implementation prompt and architecture docs
- **Contributing**: Follow the patterns established in existing features

## References

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Docker Documentation](https://docs.docker.com/)
- [VS Code Dev Containers](https://code.visualstudio.com/docs/devcontainers/containers)
