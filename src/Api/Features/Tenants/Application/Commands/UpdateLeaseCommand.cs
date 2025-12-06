using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Tenants.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Tenants.Application.Commands;

public sealed record UpdateLeaseCommand(
    Guid Id,
    DateOnly StartDate,
    DateOnly? EndDate,
    decimal MonthlyRent,
    string Currency,
    decimal? DepositAmount,
    int RentDayOfMonth,
    string? Notes) : IRequest<LeaseDto>;

internal sealed class UpdateLeaseCommandValidator : AbstractValidator<UpdateLeaseCommand>
{
    public UpdateLeaseCommandValidator()
    {
        RuleFor(x => x.Id)
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

internal sealed class UpdateLeaseCommandHandler : IRequestHandler<UpdateLeaseCommand, LeaseDto>
{
    private readonly PropertyManagementDbContext _context;

    public UpdateLeaseCommandHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<LeaseDto> Handle(UpdateLeaseCommand request, CancellationToken cancellationToken)
    {
        var lease = await _context.Leases
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Lease with ID {request.Id} not found");

        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == lease.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Tenant with ID {lease.TenantId} not found");

        lease.Update(
            request.StartDate,
            request.EndDate,
            request.MonthlyRent,
            request.Currency,
            request.DepositAmount,
            request.RentDayOfMonth,
            request.Notes);

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
