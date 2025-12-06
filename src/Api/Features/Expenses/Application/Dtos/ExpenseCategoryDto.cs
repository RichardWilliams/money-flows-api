namespace PropertyManagement.Api.Features.Expenses.Application.Dtos;

public sealed record ExpenseCategoryDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}
