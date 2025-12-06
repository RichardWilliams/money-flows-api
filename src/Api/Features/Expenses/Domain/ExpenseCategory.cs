namespace PropertyManagement.Api.Features.Expenses.Domain;

public sealed class ExpenseCategory
{
    public Guid Id { get; init; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public static ExpenseCategory Create(
        string name,
        string code,
        string? description = null)
    {
        return new ExpenseCategory
        {
            Id = Guid.NewGuid(),
            Name = name,
            Code = code.ToUpperInvariant(),
            Description = description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string code, string? description)
    {
        Name = name;
        Code = code.ToUpperInvariant();
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
