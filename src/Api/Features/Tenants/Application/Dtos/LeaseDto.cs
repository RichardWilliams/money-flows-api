using PropertyManagement.Api.Features.Tenants.Domain;

namespace PropertyManagement.Api.Features.Tenants.Application.Dtos;

public sealed record LeaseDto
{
    public Guid Id { get; init; }
    public Guid PropertyId { get; init; }
    public Guid TenantId { get; init; }
    public required string TenantName { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public decimal MonthlyRent { get; init; }
    public required string Currency { get; init; }
    public decimal? DepositAmount { get; init; }
    public int RentDayOfMonth { get; init; }
    public LeaseStatus Status { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
