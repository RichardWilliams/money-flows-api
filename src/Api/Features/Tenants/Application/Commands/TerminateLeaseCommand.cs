using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Tenants.Application.Commands;

public sealed record TerminateLeaseCommand(Guid Id, DateOnly EndDate) : IRequest;

internal sealed class TerminateLeaseCommandValidator : AbstractValidator<TerminateLeaseCommand>
{
    public TerminateLeaseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty();
    }
}

internal sealed class TerminateLeaseCommandHandler : IRequestHandler<TerminateLeaseCommand>
{
    private readonly PropertyManagementDbContext _context;

    public TerminateLeaseCommandHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task Handle(TerminateLeaseCommand request, CancellationToken cancellationToken)
    {
        var lease = await _context.Leases
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Lease with ID {request.Id} not found");

        lease.Terminate(request.EndDate);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
