using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Expenses.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Expenses.Application.Queries;

public sealed record GetExpenseQuery(Guid Id) : IRequest<ExpenseDto>;

internal sealed class GetExpenseQueryValidator : AbstractValidator<GetExpenseQuery>
{
    public GetExpenseQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal sealed class GetExpenseQueryHandler : IRequestHandler<GetExpenseQuery, ExpenseDto>
{
    private readonly PropertyManagementDbContext _context;

    public GetExpenseQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseDto> Handle(GetExpenseQuery request, CancellationToken cancellationToken)
    {
        var expense = await _context.Expenses
            .AsNoTracking()
            .Where(e => e.Id == request.Id)
            .Select(e => new
            {
                Expense = e,
                Category = _context.ExpenseCategories
                    .Where(c => c.Id == e.CategoryId)
                    .Select(c => c.Name)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Expense with ID {request.Id} not found");

        return new ExpenseDto
        {
            Id = expense.Expense.Id,
            PropertyId = expense.Expense.PropertyId,
            CategoryId = expense.Expense.CategoryId,
            CategoryName = expense.Category ?? "Unknown",
            Description = expense.Expense.Description,
            Amount = expense.Expense.Amount,
            Currency = expense.Expense.Currency,
            Date = expense.Expense.Date,
            Vendor = expense.Expense.Vendor,
            Reference = expense.Expense.Reference,
            Notes = expense.Expense.Notes,
            CreatedAt = expense.Expense.CreatedAt,
            UpdatedAt = expense.Expense.UpdatedAt
        };
    }
}
