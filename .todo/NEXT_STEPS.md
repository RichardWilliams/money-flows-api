# Next Steps for Money Flows API Development

## Phase 1: Foundation & Infrastructure ✅ COMPLETE

All infrastructure is in place and ready for feature development.

---

## Phase 2: MoneyFlows Feature - START HERE 🔜

### Step-by-Step Implementation Guide

#### 1. Domain Layer (Start Here)

Create the core business entities and interfaces:

**Files to create:**
```
Features/MoneyFlows/Domain/
├── MoneyFlow.cs              # Immutable record with business rules
├── IMoneyFlowRepository.cs   # Repository interface
├── MoneyFlowFilter.cs        # Filter model for queries
└── MoneyFlowExceptions.cs    # Domain-specific exceptions
```

**Example MoneyFlow.cs:**
```csharp
namespace PropertyManagement.Api.Features.MoneyFlows.Domain;

internal sealed record MoneyFlow(
    int Id,
    int PropertyId,
    int MoneyFlowTypeId,
    int ParticipantId,
    decimal Amount,
    string Currency,
    DateTimeOffset TransactionDate,
    string? AttachmentPath,
    string? Description,
    int? CreatedBy,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static MoneyFlow Create(...)
    {
        // Factory method with validation
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");

        return new MoneyFlow(...);
    }
}
```

#### 2. Infrastructure Layer (Database)

Set up the database entities and repository:

**Files to create:**
```
Features/MoneyFlows/Infrastructure/Persistence/
├── MoneyFlowEntity.cs           # EF Core entity
├── MoneyFlowConfiguration.cs    # EF Core configuration
└── MoneyFlowRepository.cs       # Repository implementation
```

**Migration command:**
```bash
dotnet ef migrations add AddMoneyFlows --project src/Api
dotnet ef database update --project src/Api
```

#### 3. Application Layer (Commands & Queries)

Implement the CQRS pattern:

**Files to create:**
```
Features/MoneyFlows/Application/
├── Commands/
│   ├── CreateMoneyFlowCommand.cs    # + Handler
│   ├── UpdateMoneyFlowCommand.cs    # + Handler
│   └── DeleteMoneyFlowCommand.cs    # + Handler
├── Queries/
│   ├── GetMoneyFlowsQuery.cs        # + Handler
│   └── GetMoneyFlowDetailsQuery.cs  # + Handler
├── Dtos/
│   ├── MoneyFlowDto.cs
│   └── MoneyFlowListItemDto.cs
├── Validators/
│   ├── CreateMoneyFlowCommandValidator.cs
│   └── UpdateMoneyFlowCommandValidator.cs
└── Profiles/
    └── MoneyFlowProfile.cs           # AutoMapper profile
```

#### 4. Endpoints Layer (HTTP API)

Expose the functionality via Minimal APIs:

**Files to create:**
```
Features/MoneyFlows/Endpoints/
├── Post.cs        # Create money flow
├── Get.cs         # List money flows
├── GetDetails.cs  # Get single money flow
├── Put.cs         # Update money flow
└── Delete.cs      # Delete money flow
```

#### 5. Feature Registration

Wire everything together:

**File to create:**
```
Features/MoneyFlows/
└── Feature.cs     # Service and endpoint registration
```

**Update Program.cs:**
```csharp
// Add before app.Run():
PropertyManagement.Features.MoneyFlows.Feature.ConfigureEndpoints(app);
```

#### 6. Testing

Write comprehensive tests:

**Files to create:**
```
Api.Tests/Features/MoneyFlows/
├── Domain/
│   └── MoneyFlowTests.cs
├── Commands/
│   ├── CreateMoneyFlowCommandHandlerTests.cs
│   ├── UpdateMoneyFlowCommandHandlerTests.cs
│   └── DeleteMoneyFlowCommandHandlerTests.cs
├── Queries/
│   ├── GetMoneyFlowsQueryHandlerTests.cs
│   └── GetMoneyFlowDetailsQueryHandlerTests.cs
└── Endpoints/
    └── MoneyFlowsEndpointTests.cs
```

---

## Development Workflow

### Daily Development Loop

1. **Start the devcontainer**
   ```bash
   code .
   # Reopen in Container
   ```

2. **Run in watch mode**
   ```bash
   dotnet watch run --project src/Api
   ```

3. **Run tests in watch mode** (separate terminal)
   ```bash
   dotnet watch test
   ```

4. **Test with Swagger**
   - Visit: http://localhost:5000/swagger
   - Test endpoints interactively

5. **Check health**
   - Visit: http://localhost:5000/health

### Before Each Commit

```bash
# Format code
dotnet format

# Run all tests
dotnet test

# Check build
dotnet build --configuration Release

# Review changes
git status
git diff
```

### Creating Migrations

```bash
# Create migration
dotnet ef migrations add <MigrationName> --project src/Api

# Review migration
cat src/Api/Migrations/<timestamp>_<MigrationName>.cs

# Apply migration
dotnet ef database update --project src/Api

# If needed, rollback
dotnet ef database update <PreviousMigration> --project src/Api
```

---

## Key Patterns to Follow

### 1. Vertical Slice Architecture

Each feature is self-contained:
- ✅ All related code in one folder
- ✅ Domain, Application, Infrastructure, Endpoints
- ✅ Easy to find and modify

### 2. CQRS Pattern

Separate reads from writes:
- ✅ Commands for mutations (Create, Update, Delete)
- ✅ Queries for reads (Get, List)
- ✅ Different DTOs for different use cases

### 3. Immutable Domain Models

Use C# records:
- ✅ Immutable by default
- ✅ Value equality
- ✅ `with` expressions for updates

### 4. Validation

Use FluentValidation:
- ✅ Declarative validation rules
- ✅ Automatic validation via pipeline
- ✅ Clear error messages

### 5. Repository Pattern

Abstract data access:
- ✅ Interface in Domain layer
- ✅ Implementation in Infrastructure layer
- ✅ Easy to mock for testing

### 6. Unit of Work

Coordinate transactions:
- ✅ Single SaveChanges call
- ✅ Automatic via pipeline behavior
- ✅ Rollback on exceptions

---

## File Attachment Implementation

### Date-First Organization

Files stored as:
```
/attachments/
  {year}/                    # 2025
    {month}/                 # 01, 02, ..., 12
      {property}/            # cottage-lane, seaside-retreat
        {participant}-{type}-{day}-{count}.{ext}
```

### Implementation Steps

1. **Calculate path** in command handler
2. **Save to database** with path
3. **Save physical file** after successful DB commit
4. **Handle duplicates** with counter suffix

### Example Path Calculation

```csharp
private static string CalculateAttachmentPath(
    IFormFile file,
    AttachmentSettings settings,
    DateTimeOffset date,
    string propertyName,
    string participantName,
    string typeName)
{
    var extension = Path.GetExtension(file.FileName);
    var basePath = Path.Combine(
        settings.BasePath,
        date.Year.ToString(),
        date.Month.ToString("00"),
        SanitizeName(propertyName));

    var fileName = $"{SanitizeName(participantName)}-{SanitizeName(typeName)}-{date.Day:00}";

    // Handle duplicates
    var counter = 1;
    string fullPath;
    do {
        fullPath = Path.Combine(basePath, $"{fileName}-{counter}{extension}");
        counter++;
    } while (File.Exists(fullPath));

    return fullPath;
}

private static string SanitizeName(string name)
    => Regex.Replace(name, @"[^a-zA-Z0-9]", "-").ToLowerInvariant();
```

---

## Testing Strategy

### Unit Tests (80%+ Coverage)

Test handlers with mocked dependencies:

```csharp
public class CreateMoneyFlowCommandHandlerTests
{
    private readonly Mock<IMoneyFlowRepository> _mockRepo;
    private readonly CreateMoneyFlowCommandHandler _handler;

    public CreateMoneyFlowCommandHandlerTests()
    {
        _mockRepo = new Mock<IMoneyFlowRepository>();
        _handler = new CreateMoneyFlowCommandHandler(_mockRepo.Object, ...);
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesMoneyFlow()
    {
        // Arrange
        var command = new CreateMoneyFlowCommand(...);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _mockRepo.Verify(x => x.AddAsync(...), Times.Once);
    }
}
```

### Integration Tests

Test endpoints with real database:

```csharp
public class MoneyFlowsEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task PostMoneyFlow_WithValidData_ReturnsCreated()
    {
        var client = factory.CreateClient();
        var formContent = new MultipartFormDataContent { ... };

        var response = await client.PostAsync("/api/moneyflows", formContent);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

---

## Documentation Requirements

### 1. XML Comments

Add XML comments to public APIs:

```csharp
/// <summary>
/// Create a new money flow transaction
/// </summary>
/// <param name="command">The create command</param>
/// <returns>Created money flow</returns>
public async Task<MoneyFlowDto> Handle(CreateMoneyFlowCommand command, ...)
{
    // ...
}
```

### 2. Swagger Documentation

Endpoints automatically documented via XML comments and OpenAPI attributes:

```csharp
builder.MapPost("api/moneyflows", async (...) => { ... })
    .Produces<MoneyFlowDto>(StatusCodes.Status201Created)
    .ProducesValidationProblem()
    .WithName("CreateMoneyFlow")
    .WithTags("MoneyFlows")
    .WithOpenApi();
```

### 3. Update CHANGELOG.md

Document all changes:

```markdown
## [Unreleased]

### Added
- MoneyFlows CRUD endpoints
- File attachment support
- Date-first file organization
- Filtering and pagination
```

---

## Success Criteria for Phase 2

- [ ] All CRUD operations working
- [ ] File attachments with date-first organization
- [ ] Filtering by property, participant, type, date, amount
- [ ] Pagination working correctly
- [ ] 80%+ test coverage
- [ ] All tests passing
- [ ] Swagger documentation complete
- [ ] No compiler warnings
- [ ] Code formatted with `dotnet format`
- [ ] CHANGELOG.md updated

---

## Common Commands

```bash
# Build
dotnet build

# Run
dotnet run --project src/Api

# Test
dotnet test

# Watch (auto-reload)
dotnet watch run --project src/Api
dotnet watch test

# Format
dotnet format

# Migration
dotnet ef migrations add <Name> --project src/Api
dotnet ef database update --project src/Api

# Clean
dotnet clean
```

---

## Getting Help

- **Architecture**: See [docs/ARCHITECTURE.md](../docs/ARCHITECTURE.md)
- **Setup Issues**: See [docs/SETUP.md](../docs/SETUP.md)
- **API Reference**: See [docs/API.md](../docs/API.md)
- **Full Requirements**: See [../../files/CLAUDE_CODE_IMPLEMENTATION_PROMPT.md](../../files/CLAUDE_CODE_IMPLEMENTATION_PROMPT.md)

---

## Ready to Start?

1. ✅ Review Phase 1 implementation
2. ✅ Understand the patterns
3. ✅ Set up your environment
4. 🚀 **Start with MoneyFlow domain model**
5. 🚀 **Build layer by layer**
6. 🚀 **Test as you go**
7. 🚀 **Document everything**

**Good luck with Phase 2!** 🎉
