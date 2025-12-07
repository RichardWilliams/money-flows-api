using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Tenants.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Tenants.Application.Commands;

public sealed record UpdateTenantCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? EmergencyContact,
    string? EmergencyPhone,
    string? Notes) : IRequest<TenantDto>;

internal sealed class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .Matches(@"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Must be a valid UK mobile number");

        RuleFor(x => x.EmergencyContact)
            .MaximumLength(200);

        RuleFor(x => x.EmergencyPhone)
            .MaximumLength(20);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}

internal sealed class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, TenantDto>
{
    private readonly PropertyManagementDbContext _context;

    public UpdateTenantCommandHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<TenantDto> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Tenant with ID {request.Id} not found");

        tenant.Update(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.EmergencyContact,
            request.EmergencyPhone,
            request.Notes);

        await _context.SaveChangesAsync(cancellationToken);

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
