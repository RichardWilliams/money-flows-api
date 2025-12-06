using PropertyManagement.Api.Features.Properties.Domain;

namespace PropertyManagement.Api.Features.Properties.Application.Dtos;

public sealed record PropertyListDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string City { get; init; }
    public required string Postcode { get; init; }
    public required PropertyType Type { get; init; }
    public required PropertyStatus Status { get; init; }
    public int Bedrooms { get; init; }
    public int Bathrooms { get; init; }
}
