using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Tenants.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Tenants.Application.Queries;

public sealed record GetLeaseQuery(Guid Id) : IRequest<LeaseDto>;

internal sealed class GetLeaseQueryValidator : AbstractValidator<GetLeaseQuery>
{
    public GetLeaseQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal sealed class GetLeaseQueryHandler : IRequestHandler<GetLeaseQuery, LeaseDto>
{
    private readonly PropertyManagementDbContext _context;

    public GetLeaseQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<LeaseDto> Handle(GetLeaseQuery request, CancellationToken cancellationToken)
    {
        var lease = await _context.Leases
            .AsNoTracking()
            .Where(l => l.Id == request.Id)
            .Select(l => new
            {
                Lease = l,
                TenantName = _context.Tenants
                    .Where(t => t.Id == l.TenantId)
                    .Select(t => t.FirstName + " " + t.LastName)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Lease with ID {request.Id} not found");

        return new LeaseDto
        {
            Id = lease.Lease.Id,
            PropertyId = lease.Lease.PropertyId,
            TenantId = lease.Lease.TenantId,
            TenantName = lease.TenantName ?? "Unknown",
            StartDate = lease.Lease.StartDate,
            EndDate = lease.Lease.EndDate,
            MonthlyRent = lease.Lease.MonthlyRent,
            Currency = lease.Lease.Currency,
            DepositAmount = lease.Lease.DepositAmount,
            RentDayOfMonth = lease.Lease.RentDayOfMonth,
            Status = lease.Lease.Status,
            Notes = lease.Lease.Notes,
            CreatedAt = lease.Lease.CreatedAt,
            UpdatedAt = lease.Lease.UpdatedAt
        };
    }
}
