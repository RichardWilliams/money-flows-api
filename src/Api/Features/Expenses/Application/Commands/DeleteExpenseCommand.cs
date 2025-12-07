using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Expenses.Application.Commands;

public sealed record DeleteExpenseCommand(Guid Id) : IRequest;

internal sealed class DeleteExpenseCommandValidator : AbstractValidator<DeleteExpenseCommand>
{
    public DeleteExpenseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal sealed class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand>
{
    private readonly PropertyManagementDbContext _context;

    public DeleteExpenseCommandHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await _context.Expenses
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Expense with ID {request.Id} not found");

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
