using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Expenses.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Models;

namespace PropertyManagement.Api.Features.Expenses.Application.Queries;

public sealed record ListExpensesQuery(
    Guid? PropertyId = null,
    Guid? CategoryId = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedList<ExpenseListDto>>;

internal sealed class ListExpensesQueryValidator : AbstractValidator<ListExpensesQuery>
{
    public ListExpensesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("ToDate must be greater than or equal to FromDate");
    }
}

internal sealed class ListExpensesQueryHandler : IRequestHandler<ListExpensesQuery, PagedList<ExpenseListDto>>
{
    private readonly PropertyManagementDbContext _context;

    public ListExpensesQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<ExpenseListDto>> Handle(ListExpensesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Expenses.AsNoTracking();

        if (request.PropertyId.HasValue)
        {
            query = query.Where(e => e.PropertyId == request.PropertyId.Value);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == request.CategoryId.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(e => e.Date >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(e => e.Date <= request.ToDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var expenses = await query
            .OrderByDescending(e => e.Date)
            .ThenBy(e => e.Description)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new ExpenseListDto
            {
                Id = e.Id,
                PropertyId = e.PropertyId,
                CategoryName = _context.ExpenseCategories
                    .Where(c => c.Id == e.CategoryId)
                    .Select(c => c.Name)
                    .FirstOrDefault() ?? "Unknown",
                Description = e.Description,
                Amount = e.Amount,
                Currency = e.Currency,
                Date = e.Date,
                Vendor = e.Vendor
            })
            .ToListAsync(cancellationToken);

        return PagedList<ExpenseListDto>.Create(
            expenses,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
