using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Tenants.Application.Dtos;
using PropertyManagement.Api.Features.Tenants.Domain;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Tenants.Application.Commands;

public sealed record CreateLeaseCommand(
    Guid PropertyId,
    Guid TenantId,
    DateOnly StartDate,
    DateOnly? EndDate,
    decimal MonthlyRent,
    string Currency,
    decimal? DepositAmount,
    int RentDayOfMonth,
    string? Notes) : IRequest<LeaseDto>;

internal sealed class CreateLeaseCommandValidator : AbstractValidator<CreateLeaseCommand>
{
    public CreateLeaseCommandValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty();

        RuleFor(x => x.TenantId)
            .NotEmpty();

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.MonthlyRent)
            .GreaterThan(0)
            .LessThan(100000);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("Currency must be a 3-letter ISO code (e.g., GBP, EUR, USD)");

        RuleFor(x => x.DepositAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DepositAmount.HasValue);

        RuleFor(x => x.RentDayOfMonth)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(28)
            .WithMessage("Rent day must be between 1 and 28");

        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}

internal sealed class CreateLeaseCommandHandler : IRequestHandler<CreateLeaseCommand, LeaseDto>
{
    private readonly PropertyManagementDbContext _context;

    public CreateLeaseCommandHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<LeaseDto> Handle(CreateLeaseCommand request, CancellationToken cancellationToken)
    {
        var propertyExists = await _context.Properties
            .AnyAsync(p => p.Id == request.PropertyId, cancellationToken);

        if (!propertyExists)
            throw new NotFoundException($"Property with ID {request.PropertyId} not found");

        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Tenant with ID {request.TenantId} not found");

        var lease = Lease.Create(
            request.PropertyId,
            request.TenantId,
            request.StartDate,
            request.EndDate,
            request.MonthlyRent,
            request.Currency,
            request.DepositAmount,
            request.RentDayOfMonth,
            request.Notes);

        _context.Leases.Add(lease);
        await _context.SaveChangesAsync(cancellationToken);

        return new LeaseDto
        {
            Id = lease.Id,
            PropertyId = lease.PropertyId,
            TenantId = lease.TenantId,
            TenantName = tenant.FullName,
            StartDate = lease.StartDate,
            EndDate = lease.EndDate,
            MonthlyRent = lease.MonthlyRent,
            Currency = lease.Currency,
            DepositAmount = lease.DepositAmount,
            RentDayOfMonth = lease.RentDayOfMonth,
            Status = lease.Status,
            Notes = lease.Notes,
            CreatedAt = lease.CreatedAt,
            UpdatedAt = lease.UpdatedAt
        };
    }
}
