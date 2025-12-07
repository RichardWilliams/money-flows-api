using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.MoneyFlows.Application.Commands;

public sealed record DeleteMoneyFlowCommand(Guid Id) : IRequest;

internal sealed class DeleteMoneyFlowCommandValidator : AbstractValidator<DeleteMoneyFlowCommand>
{
    public DeleteMoneyFlowCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

internal sealed class DeleteMoneyFlowCommandHandler : IRequestHandler<DeleteMoneyFlowCommand>
{
    private readonly PropertyManagementDbContext _context;

    public DeleteMoneyFlowCommandHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteMoneyFlowCommand request, CancellationToken cancellationToken)
    {
        var moneyFlow = await _context.MoneyFlows
            .FirstOrDefaultAsync(mf => mf.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"MoneyFlow with ID {request.Id} not found");

        _context.MoneyFlows.Remove(moneyFlow);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
