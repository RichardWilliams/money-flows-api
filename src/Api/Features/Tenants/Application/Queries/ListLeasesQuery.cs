using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Tenants.Application.Dtos;
using PropertyManagement.Api.Features.Tenants.Domain;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Models;

namespace PropertyManagement.Api.Features.Tenants.Application.Queries;

public sealed record ListLeasesQuery(
    Guid? PropertyId = null,
    Guid? TenantId = null,
    LeaseStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedList<LeaseDto>>;

internal sealed class ListLeasesQueryValidator : AbstractValidator<ListLeasesQuery>
{
    public ListLeasesQueryValidator()
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

internal sealed class ListLeasesQueryHandler : IRequestHandler<ListLeasesQuery, PagedList<LeaseDto>>
{
    private readonly PropertyManagementDbContext _context;

    public ListLeasesQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<LeaseDto>> Handle(ListLeasesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Leases.AsNoTracking();

        if (request.PropertyId.HasValue)
        {
            query = query.Where(l => l.PropertyId == request.PropertyId.Value);
        }

        if (request.TenantId.HasValue)
        {
            query = query.Where(l => l.TenantId == request.TenantId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(l => l.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var leases = await query
            .OrderByDescending(l => l.StartDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new LeaseDto
            {
                Id = l.Id,
                PropertyId = l.PropertyId,
                TenantId = l.TenantId,
                TenantName = _context.Tenants
                    .Where(t => t.Id == l.TenantId)
                    .Select(t => t.FirstName + " " + t.LastName)
                    .FirstOrDefault() ?? "Unknown",
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                MonthlyRent = l.MonthlyRent,
                Currency = l.Currency,
                DepositAmount = l.DepositAmount,
                RentDayOfMonth = l.RentDayOfMonth,
                Status = l.Status,
                Notes = l.Notes,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return PagedList<LeaseDto>.Create(
            leases,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
