namespace PropertyManagement.Api.Features.Expenses.Domain;

public sealed class Expense
{
    public Guid Id { get; init; }
    public Guid PropertyId { get; set; }
    public Guid CategoryId { get; set; }
    public required string Description { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public DateOnly Date { get; set; }
    public string? Vendor { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public static Expense Create(
        Guid propertyId,
        Guid categoryId,
        string description,
        decimal amount,
        string currency,
        DateOnly date,
        string? vendor = null,
        string? reference = null,
        string? notes = null)
    {
        return new Expense
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            CategoryId = categoryId,
            Description = description,
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            Date = date,
            Vendor = vendor,
            Reference = reference,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        Guid categoryId,
        string description,
        decimal amount,
        string currency,
        DateOnly date,
        string? vendor,
        string? reference,
        string? notes)
    {
        CategoryId = categoryId;
        Description = description;
        Amount = amount;
        Currency = currency.ToUpperInvariant();
        Date = date;
        Vendor = vendor;
        Reference = reference;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
