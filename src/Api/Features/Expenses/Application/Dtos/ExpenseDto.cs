namespace PropertyManagement.Api.Features.Expenses.Application.Dtos;

public sealed record ExpenseDto
{
    public Guid Id { get; init; }
    public Guid PropertyId { get; init; }
    public Guid CategoryId { get; init; }
    public required string CategoryName { get; init; }
    public required string Description { get; init; }
    public decimal Amount { get; init; }
    public required string Currency { get; init; }
    public DateOnly Date { get; init; }
    public string? Vendor { get; init; }
    public string? Reference { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
