# Architecture Documentation

## Overview

The UK Property Management API follows **Vertical Slice Architecture** combined with **CQRS (Command Query Responsibility Segregation)** pattern. This architectural approach promotes high cohesion, low coupling, and makes the codebase easy to navigate and maintain.

## Architectural Principles

### 1. Vertical Slice Architecture

Each feature is self-contained in its own vertical slice:

```
Features/MoneyFlows/
├── Domain/              # Business logic & entities
├── Application/         # Commands, Queries, DTOs
├── Infrastructure/      # Data access & external services
├── Endpoints/           # HTTP endpoints
└── Feature.cs           # Registration
```

**Benefits:**
- Features are independent and can be developed in parallel
- Easy to locate all code related to a feature
- Changes are localized to a single slice
- New team members can understand one slice without knowing the whole system

### 2. CQRS with MediatR

All business operations are expressed as either:

- **Commands** - Operations that change state (Create, Update, Delete)
- **Queries** - Operations that read state (Get, List, Search)

```csharp
// Command
public record CreateMoneyFlowCommand(...) : IRequest<MoneyFlowDto>;

// Query
public record GetMoneyFlowsQuery(...) : IRequest<PagedList<MoneyFlowDto>>;
```

**Benefits:**
- Clear separation of reads and writes
- Optimized query models
- Easy to add cross-cutting concerns via pipeline behaviors
- Testable in isolation

### 3. Pipeline Behaviors

MediatR pipeline behaviors provide cross-cutting concerns:

```
Request → Logging → Validation → Transaction → Handler → Response
```

**Behaviors:**
1. **Logging** - Request/response logging with timing
2. **Validation** - FluentValidation automatic validation
3. **Transaction** - Automatic transaction management for commands

### 4. Repository Pattern + Unit of Work

Data access is abstracted through repositories:

```csharp
// Domain interface (feature-specific)
public interface IMoneyFlowRepository
{
    Task<MoneyFlow> AddAsync(MoneyFlow moneyFlow, CancellationToken ct);
    Task<MoneyFlow?> GetByIdAsync(int id, CancellationToken ct);
    // ... other methods
}

// Infrastructure implementation
internal sealed class MoneyFlowRepository : IMoneyFlowRepository
{
    // EF Core implementation
}

// Unit of Work for transaction coordination
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
}
```

**Benefits:**
- Testable without database
- Clean separation of domain and infrastructure
- Easy to swap implementations
- Transaction management in one place

## Layer Responsibilities

### Domain Layer

**Purpose:** Pure business logic, no dependencies on infrastructure

**Contains:**
- Domain entities (as C# records)
- Domain interfaces (repositories, services)
- Domain exceptions
- Business rules and invariants

**Example:**
```csharp
internal sealed record MoneyFlow(
    int Id,
    int PropertyId,
    decimal Amount,
    DateTimeOffset TransactionDate,
    // ... other properties
)
{
    public static MoneyFlow Create(...)
    {
        // Factory method with business rules
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");

        return new MoneyFlow(...);
    }
}
```

### Application Layer

**Purpose:** Orchestrate business operations, no infrastructure details

**Contains:**
- Commands and CommandHandlers
- Queries and QueryHandlers
- DTOs (Data Transfer Objects)
- Validators (FluentValidation)
- AutoMapper Profiles

**Example:**
```csharp
// Command
internal sealed record CreateMoneyFlowCommand(...) : IRequest<MoneyFlowDto>;

// Handler
internal sealed class CreateMoneyFlowCommandHandler
    : IRequestHandler<CreateMoneyFlowCommand, MoneyFlowDto>
{
    public async Task<MoneyFlowDto> Handle(CreateMoneyFlowCommand request, ...)
    {
        // Orchestrate domain logic
        // No SQL, no HTTP, no file I/O directly
    }
}

// Validator
internal sealed class CreateMoneyFlowCommandValidator
    : AbstractValidator<CreateMoneyFlowCommand>
{
    public CreateMoneyFlowCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PropertyId).GreaterThan(0);
    }
}
```

### Infrastructure Layer

**Purpose:** External concerns (database, files, APIs)

**Contains:**
- Repository implementations
- Entity Framework configurations
- Database entities (separate from domain entities)
- Mappers (domain ↔ database entities)
- External service clients

**Example:**
```csharp
// EF Core entity (infrastructure concern)
internal sealed class MoneyFlowEntity
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public decimal Amount { get; set; }
    // ... EF Core specific properties
}

// Configuration
internal sealed class MoneyFlowConfiguration
    : IEntityTypeConfiguration<MoneyFlowEntity>
{
    public void Configure(EntityTypeBuilder<MoneyFlowEntity> builder)
    {
        builder.ToTable("money_flows", "property_management");
        builder.HasKey(e => e.Id);
        // ... other configurations
    }
}
```

### Endpoints Layer

**Purpose:** HTTP API surface, minimal logic

**Contains:**
- Minimal API endpoint definitions
- Route configuration
- Response mapping

**Example:**
```csharp
internal static class Post
{
    public static void Configure(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/moneyflows", async (
            IMediator mediator,
            CreateMoneyFlowCommand command,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/moneyflows/{result.Id}", result);
        })
        .Produces<MoneyFlowDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .WithName("CreateMoneyFlow")
        .WithTags("MoneyFlows")
        .WithOpenApi();
    }
}
```

## Cross-Cutting Concerns

### Exception Handling

Global exception handler in `Program.cs`:

```csharp
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

    var problemDetails = exception switch
    {
        ValidationException => /* 400 Bad Request */,
        NotFoundException => /* 404 Not Found */,
        _ => /* 500 Internal Server Error */
    };

    await context.Response.WriteAsJsonAsync(problemDetails);
}));
```

### Validation

Automatic validation via pipeline behavior:

```csharp
// ValidationBehavior runs before every handler
internal sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(...)
    {
        // Run all validators for this request
        // Throw ValidationException if any fail
        // Otherwise proceed to next behavior/handler
    }
}
```

### Logging

Structured logging with Serilog:

```csharp
// Automatically logs every request
logger.LogInformation(
    "Handling {RequestName}: {@Request}",
    requestName,
    request);

// Automatically logs timing
logger.LogInformation(
    "Handled {RequestName} in {ElapsedMs}ms",
    requestName,
    stopwatch.ElapsedMilliseconds);
```

### Transactions

Automatic transaction management:

```csharp
// TransactionBehavior wraps commands in transactions
internal sealed class TransactionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(...)
    {
        // Skip queries
        if (typeof(TRequest).Name.EndsWith("Query"))
            return await next();

        // Wrap commands in transaction
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var response = await next();
            await transaction.CommitAsync();
            return response;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

## Database Architecture

### Dual Schema Design

```sql
-- New application schema (default)
CREATE SCHEMA property_management;

-- Legacy schema (existing data)
CREATE SCHEMA home_accounts;

-- Migration views
CREATE VIEW property_management.legacy_money_flows AS
SELECT * FROM home_accounts.money_flows;
```

**Migration Strategy:**
1. New app reads/writes to `property_management` schema
2. Views provide access to legacy data when needed
3. Background job gradually migrates data
4. Once complete, drop views and old schema
5. Zero downtime, zero data loss

### Entity Framework Configuration

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Set default schema
    modelBuilder.HasDefaultSchema("property_management");

    // Apply all configurations
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);

    // Create legacy schema reference
    modelBuilder.HasSequence("money_flow_id_seq", "home_accounts");
}
```

## Testing Strategy

### Unit Tests

Test handlers in isolation with mocked dependencies:

```csharp
public class CreateMoneyFlowCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_CreatesMoneyFlow()
    {
        // Arrange
        var mockRepo = new Mock<IMoneyFlowRepository>();
        var handler = new CreateMoneyFlowCommandHandler(mockRepo.Object, ...);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        mockRepo.Verify(x => x.AddAsync(...), Times.Once);
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
        var response = await client.PostAsync("/api/moneyflows", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

## Security Considerations

1. **Input Validation** - FluentValidation on all commands
2. **SQL Injection** - Protected by EF Core parameterization
3. **XSS** - JSON serialization escapes dangerous characters
4. **CORS** - Configured allowed origins only
5. **File Uploads** - Type and size restrictions
6. **Authentication** - To be added in Phase 7

## Performance Considerations

1. **Database Indexes** - On common query fields
2. **Pagination** - All list endpoints are paginated
3. **Connection Pooling** - EF Core connection pool
4. **Retry Logic** - Database retry on transient failures
5. **Async/Await** - Throughout the stack

## Deployment Architecture

```
[Frontend (React)] → [API (.NET 10)] → [PostgreSQL 16]
                           ↓
                    [Serilog Logs] → [Log Aggregation]
                           ↓
                    [Health Checks] → [Monitoring]
```

**Cloud Deployment:**
- Frontend: Vercel/Netlify
- API: Railway/Fly.io/Azure App Service
- Database: Supabase/Neon/Azure PostgreSQL
- Logs: CloudWatch/Application Insights
- Monitoring: Health check endpoints

## Future Enhancements

1. **Caching** - Redis for frequently accessed data
2. **Background Jobs** - Hangfire for data migration and scheduled tasks
3. **Message Queue** - RabbitMQ for platform integrations
4. **Event Sourcing** - For audit trail and temporal queries
5. **CQRS Read Models** - Optimized query databases

## References

- [Vertical Slice Architecture](https://jimmybogard.com/vertical-slice-architecture/)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
