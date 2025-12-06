using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Tenants.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Tenants.Application.Queries;

public sealed record GetTenantQuery(Guid Id) : IRequest<TenantDto>;

internal sealed class GetTenantQueryValidator : AbstractValidator<GetTenantQuery>
{
    public GetTenantQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal sealed class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, TenantDto>
{
    private readonly PropertyManagementDbContext _context;

    public GetTenantQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<TenantDto> Handle(GetTenantQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Tenant with ID {request.Id} not found");

        return new TenantDto
        {
            Id = tenant.Id,
            FirstName = tenant.FirstName,
            LastName = tenant.LastName,
            FullName = tenant.FullName,
            Email = tenant.Email,
            Phone = tenant.Phone,
            EmergencyContact = tenant.EmergencyContact,
            EmergencyPhone = tenant.EmergencyPhone,
            Notes = tenant.Notes,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt
        };
    }
}
