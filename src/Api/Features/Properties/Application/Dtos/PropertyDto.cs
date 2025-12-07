using PropertyManagement.Api.Features.Properties.Domain;

namespace PropertyManagement.Api.Features.Properties.Application.Dtos;

public sealed record PropertyDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public required string City { get; init; }
    public required string County { get; init; }
    public required string Postcode { get; init; }
    public required PropertyType Type { get; init; }
    public required PropertyStatus Status { get; init; }
    public int Bedrooms { get; init; }
    public int Bathrooms { get; init; }
    public decimal? PurchasePrice { get; init; }
    public DateOnly? PurchaseDate { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
