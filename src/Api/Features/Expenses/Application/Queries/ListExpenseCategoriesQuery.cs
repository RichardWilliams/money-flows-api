using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Expenses.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;

namespace PropertyManagement.Api.Features.Expenses.Application.Queries;

public sealed record ListExpenseCategoriesQuery(bool? IsActive = null) : IRequest<List<ExpenseCategoryDto>>;

internal sealed class ListExpenseCategoriesQueryHandler : IRequestHandler<ListExpenseCategoriesQuery, List<ExpenseCategoryDto>>
{
    private readonly PropertyManagementDbContext _context;

    public ListExpenseCategoriesQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExpenseCategoryDto>> Handle(ListExpenseCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ExpenseCategories.AsNoTracking();

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new ExpenseCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Description = c.Description,
                IsActive = c.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}
