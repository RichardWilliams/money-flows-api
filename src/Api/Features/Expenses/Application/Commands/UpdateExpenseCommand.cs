using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Expenses.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Expenses.Application.Commands;

public sealed record UpdateExpenseCommand(
    Guid Id,
    Guid CategoryId,
    string Description,
    decimal Amount,
    string Currency,
    DateOnly Date,
    string? Vendor,
    string? Reference,
    string? Notes) : IRequest<ExpenseDto>;

internal sealed class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
    public UpdateExpenseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .LessThan(1000000000);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("Currency must be a 3-letter ISO code (e.g., GBP, EUR, USD)");

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
            .WithMessage("Date cannot be in the future");

        RuleFor(x => x.Vendor)
            .MaximumLength(200);

        RuleFor(x => x.Reference)
            .MaximumLength(100);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}

internal sealed class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, ExpenseDto>
{
    private readonly PropertyManagementDbContext _context;

    public UpdateExpenseCommandHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseDto> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await _context.Expenses
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Expense with ID {request.Id} not found");

        var category = await _context.ExpenseCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Expense category with ID {request.CategoryId} not found");

        expense.Update(
            request.CategoryId,
            request.Description,
            request.Amount,
            request.Currency,
            request.Date,
            request.Vendor,
            request.Reference,
            request.Notes);

        await _context.SaveChangesAsync(cancellationToken);

        return new ExpenseDto
        {
            Id = expense.Id,
            PropertyId = expense.PropertyId,
            CategoryId = expense.CategoryId,
            CategoryName = category.Name,
            Description = expense.Description,
            Amount = expense.Amount,
            Currency = expense.Currency,
            Date = expense.Date,
            Vendor = expense.Vendor,
            Reference = expense.Reference,
            Notes = expense.Notes,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }
}
