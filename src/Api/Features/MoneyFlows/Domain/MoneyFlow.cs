namespace PropertyManagement.Api.Features.MoneyFlows.Domain;

public sealed class MoneyFlow
{
    public Guid Id { get; init; }
    public Guid PropertyId { get; init; }
    public MoneyFlowType Type { get; init; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public DateOnly Date { get; set; }
    public required string Description { get; set; }

    // For Expenses - link to expense category
    public Guid? ExpenseCategoryId { get; set; }

    // For Income - source description
    public string? IncomeSource { get; set; }

    // Optional relationships
    public Guid? TenantId { get; set; }
    public Guid? LeaseId { get; set; }

    // Reference numbers
    public string? Reference { get; set; }
    public string? Notes { get; set; }

    // Audit
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public static MoneyFlow Create(
        Guid propertyId,
        MoneyFlowType type,
        decimal amount,
        string currency,
        DateOnly date,
        string description,
        Guid? expenseCategoryId = null,
        string? incomeSource = null,
        Guid? tenantId = null,
        Guid? leaseId = null,
        string? reference = null,
        string? notes = null)
    {
        return new MoneyFlow
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            Type = type,
            Amount = amount,
            Currency = currency,
            Date = date,
            Description = description,
            ExpenseCategoryId = expenseCategoryId,
            IncomeSource = incomeSource,
            TenantId = tenantId,
            LeaseId = leaseId,
            Reference = reference,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        decimal amount,
        string currency,
        DateOnly date,
        string description,
        Guid? expenseCategoryId,
        string? incomeSource,
        Guid? tenantId,
        Guid? leaseId,
        string? reference,
        string? notes)
    {
        Amount = amount;
        Currency = currency;
        Date = date;
        Description = description;
        ExpenseCategoryId = expenseCategoryId;
        IncomeSource = incomeSource;
        TenantId = tenantId;
        LeaseId = leaseId;
        Reference = reference;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
