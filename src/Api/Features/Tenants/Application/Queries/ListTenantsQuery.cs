using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Tenants.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Models;

namespace PropertyManagement.Api.Features.Tenants.Application.Queries;

public sealed record ListTenantsQuery(
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedList<TenantDto>>;

internal sealed class ListTenantsQueryValidator : AbstractValidator<ListTenantsQuery>
{
    public ListTenantsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200);
    }
}

internal sealed class ListTenantsQueryHandler : IRequestHandler<ListTenantsQuery, PagedList<TenantDto>>
{
    private readonly PropertyManagementDbContext _context;

    public ListTenantsQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<TenantDto>> Handle(ListTenantsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tenants.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            query = query.Where(t =>
                t.FirstName.ToLower().Contains(searchTerm) ||
                t.LastName.ToLower().Contains(searchTerm) ||
                t.Email.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var tenants = await query
            .OrderBy(t => t.LastName)
            .ThenBy(t => t.FirstName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TenantDto
            {
                Id = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                FullName = t.FullName,
                Email = t.Email,
                Phone = t.Phone,
                EmergencyContact = t.EmergencyContact,
                EmergencyPhone = t.EmergencyPhone,
                Notes = t.Notes,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return PagedList<TenantDto>.Create(
            tenants,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
