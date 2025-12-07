namespace PropertyManagement.Api.Features.Expenses.Application.Dtos;

public sealed record ExpenseListDto
{
    public Guid Id { get; init; }
    public Guid PropertyId { get; init; }
    public required string CategoryName { get; init; }
    public required string Description { get; init; }
    public decimal Amount { get; init; }
    public required string Currency { get; init; }
    public DateOnly Date { get; init; }
    public string? Vendor { get; init; }
}
