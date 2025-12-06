using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Properties.Application.Dtos;
using PropertyManagement.Api.Features.Properties.Domain;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Models;

namespace PropertyManagement.Api.Features.Properties.Application.Queries;

public sealed record ListPropertiesQuery(
    PropertyStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedList<PropertyListDto>>;

internal sealed class ListPropertiesQueryValidator : AbstractValidator<ListPropertiesQuery>
{
    public ListPropertiesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);
    }
}

internal sealed class ListPropertiesQueryHandler : IRequestHandler<ListPropertiesQuery, PagedList<PropertyListDto>>
{
    private readonly PropertyManagementDbContext _context;

    public ListPropertiesQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<PropertyListDto>> Handle(ListPropertiesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Properties.AsNoTracking();

        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var properties = await query
            .OrderBy(p => p.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PropertyListDto
            {
                Id = p.Id,
                Name = p.Name,
                City = p.City,
                Postcode = p.Postcode,
                Type = p.Type,
                Status = p.Status,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms
            })
            .ToListAsync(cancellationToken);

        return PagedList<PropertyListDto>.Create(
            properties,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
