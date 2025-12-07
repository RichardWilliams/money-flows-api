using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.MoneyFlows.Application.Dtos;
using PropertyManagement.Api.Features.MoneyFlows.Domain;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Models;

namespace PropertyManagement.Api.Features.MoneyFlows.Application.Queries;

public sealed record ListMoneyFlowsQuery(
    Guid? PropertyId = null,
    int? Type = null,
    DateOnly? DateFrom = null,
    DateOnly? DateTo = null,
    Guid? ExpenseCategoryId = null,
    Guid? TenantId = null,
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedList<MoneyFlowListDto>>;

internal sealed class ListMoneyFlowsQueryValidator : AbstractValidator<ListMoneyFlowsQuery>
{
    public ListMoneyFlowsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("DateTo must be greater than or equal to DateFrom");

        RuleFor(x => x.Type)
            .Must(t => t == 1 || t == 2)
            .When(x => x.Type.HasValue)
            .WithMessage("Type must be 1 (Income) or 2 (Expense)");
    }
}

internal sealed class ListMoneyFlowsQueryHandler : IRequestHandler<ListMoneyFlowsQuery, PagedList<MoneyFlowListDto>>
{
    private readonly PropertyManagementDbContext _context;

    public ListMoneyFlowsQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<MoneyFlowListDto>> Handle(ListMoneyFlowsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.MoneyFlows.AsNoTracking();

        if (request.PropertyId.HasValue)
        {
            query = query.Where(mf => mf.PropertyId == request.PropertyId.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(mf => (int)mf.Type == request.Type.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(mf => mf.Date >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(mf => mf.Date <= request.DateTo.Value);
        }

        if (request.ExpenseCategoryId.HasValue)
        {
            query = query.Where(mf => mf.ExpenseCategoryId == request.ExpenseCategoryId.Value);
        }

        if (request.TenantId.HasValue)
        {
            query = query.Where(mf => mf.TenantId == request.TenantId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(mf =>
                mf.Description.ToLower().Contains(searchTerm) ||
                (mf.Reference != null && mf.Reference.ToLower().Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var moneyFlows = await query
            .OrderByDescending(mf => mf.Date)
            .ThenByDescending(mf => mf.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(mf => new MoneyFlowListDto
            {
                Id = mf.Id,
                PropertyId = mf.PropertyId,
                Type = (int)mf.Type,
                TypeName = mf.Type.ToString(),
                Amount = mf.Amount,
                Currency = mf.Currency,
                Date = mf.Date,
                Description = mf.Description,
                ExpenseCategoryName = mf.ExpenseCategoryId != null
                    ? _context.ExpenseCategories
                        .Where(c => c.Id == mf.ExpenseCategoryId.Value)
                        .Select(c => c.Name)
                        .FirstOrDefault()
                    : null,
                IncomeSource = mf.IncomeSource,
                Reference = mf.Reference
            })
            .ToListAsync(cancellationToken);

        return PagedList<MoneyFlowListDto>.Create(
            moneyFlows,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
