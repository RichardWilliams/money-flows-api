namespace PropertyManagement.Api.Features.MoneyFlows.Application.Dtos;

public sealed record MoneyFlowDto
{
    public Guid Id { get; init; }
    public Guid PropertyId { get; init; }
    public int Type { get; init; }
    public required string TypeName { get; init; }
    public decimal Amount { get; init; }
    public required string Currency { get; init; }
    public DateOnly Date { get; init; }
    public required string Description { get; init; }
    public Guid? ExpenseCategoryId { get; init; }
    public string? ExpenseCategoryName { get; init; }
    public string? IncomeSource { get; init; }
    public Guid? TenantId { get; init; }
    public Guid? LeaseId { get; init; }
    public string? Reference { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
