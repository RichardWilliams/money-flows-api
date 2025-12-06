using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Properties.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Properties.Application.Queries;

public sealed record GetPropertyQuery(Guid Id) : IRequest<PropertyDto>;

internal sealed class GetPropertyQueryValidator : AbstractValidator<GetPropertyQuery>
{
    public GetPropertyQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal sealed class GetPropertyQueryHandler : IRequestHandler<GetPropertyQuery, PropertyDto>
{
    private readonly PropertyManagementDbContext _context;

    public GetPropertyQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<PropertyDto> Handle(GetPropertyQuery request, CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Property with ID {request.Id} not found");

        return new PropertyDto
        {
            Id = property.Id,
            Name = property.Name,
            AddressLine1 = property.AddressLine1,
            AddressLine2 = property.AddressLine2,
            City = property.City,
            County = property.County,
            Postcode = property.Postcode,
            Type = property.Type,
            Status = property.Status,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            PurchasePrice = property.PurchasePrice,
            PurchaseDate = property.PurchaseDate,
            Description = property.Description,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };
    }
}
