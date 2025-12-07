using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Reports.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Reports.Application.Queries;

public sealed record GetPropertySummaryQuery(
    Guid PropertyId,
    DateOnly? DateFrom = null,
    DateOnly? DateTo = null,
    string Currency = "GBP") : IRequest<PropertySummaryDto>;

internal sealed class GetPropertySummaryQueryValidator : AbstractValidator<GetPropertySummaryQuery>
{
    public GetPropertySummaryQueryValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty()
            .WithMessage("PropertyId is required");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Must(c => new[] { "GBP", "CHF", "EUR", "USD" }.Contains(c))
            .WithMessage("Currency must be one of: GBP, CHF, EUR, USD");

        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("DateTo must be greater than or equal to DateFrom");
    }
}

internal sealed class GetPropertySummaryQueryHandler : IRequestHandler<GetPropertySummaryQuery, PropertySummaryDto>
{
    private readonly PropertyManagementDbContext _context;

    public GetPropertySummaryQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<PropertySummaryDto> Handle(GetPropertySummaryQuery request, CancellationToken cancellationToken)
    {
        // Verify property exists
        var property = await _context.Properties
            .AsNoTracking()
            .Where(p => p.Id == request.PropertyId)
            .Select(p => new { p.Id, p.Name })
            .FirstOrDefaultAsync(cancellationToken);

        if (property is null)
        {
            throw new NotFoundException($"Property with ID {request.PropertyId} not found");
        }

        // Set report period defaults
        var dateFrom = request.DateFrom ?? DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1));
        var dateTo = request.DateTo ?? DateOnly.FromDateTime(DateTime.UtcNow);

        // Get all money flows for this property in the date range
        var moneyFlows = await _context.MoneyFlows
            .AsNoTracking()
            .Where(mf => mf.PropertyId == request.PropertyId)
            .Where(mf => mf.Date >= dateFrom && mf.Date <= dateTo)
            .ToListAsync(cancellationToken);

        // Calculate income breakdown
        var incomeFlows = moneyFlows
            .Where(mf => mf.Type == Features.MoneyFlows.Domain.MoneyFlowType.Income)
            .ToList();

        var incomeBreakdown = incomeFlows
            .GroupBy(mf => mf.IncomeSource ?? "Unspecified")
            .Select(g => new IncomeSourceBreakdownDto
            {
                Source = g.Key,
                Amount = g.Sum(mf => mf.Amount),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        // Calculate expense breakdown
        var expenseFlows = moneyFlows
            .Where(mf => mf.Type == Features.MoneyFlows.Domain.MoneyFlowType.Expense)
            .ToList();

        var expenseCategoryIds = expenseFlows
            .Where(mf => mf.ExpenseCategoryId.HasValue)
            .Select(mf => mf.ExpenseCategoryId!.Value)
            .Distinct()
            .ToList();

        var expenseCategories = await _context.ExpenseCategories
            .AsNoTracking()
            .Where(c => expenseCategoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var expenseBreakdown = expenseFlows
            .Where(mf => mf.ExpenseCategoryId.HasValue)
            .GroupBy(mf => mf.ExpenseCategoryId!.Value)
            .Select(g => new ExpenseCategoryBreakdownDto
            {
                CategoryId = g.Key,
                CategoryName = expenseCategories.GetValueOrDefault(g.Key, "Unknown"),
                Amount = g.Sum(mf => mf.Amount),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        // Calculate totals
        var totalIncome = incomeFlows.Sum(mf => mf.Amount);
        var totalExpenses = expenseFlows.Sum(mf => mf.Amount);
        var netIncome = totalIncome - totalExpenses;
        var profitMargin = totalIncome > 0 ? (netIncome / totalIncome) * 100 : 0;

        return new PropertySummaryDto
        {
            PropertyId = property.Id,
            PropertyName = property.Name,
            ReportPeriod = new ReportPeriodDto
            {
                From = dateFrom,
                To = dateTo
            },
            Currency = request.Currency,
            Income = new IncomeBreakdownDto
            {
                TotalAmount = totalIncome,
                TransactionCount = incomeFlows.Count,
                Breakdown = incomeBreakdown
            },
            Expenses = new ReportExpenseBreakdownDto
            {
                TotalAmount = totalExpenses,
                TransactionCount = expenseFlows.Count,
                Breakdown = expenseBreakdown
            },
            NetIncome = netIncome,
            ProfitMargin = Math.Round(profitMargin, 2)
        };
    }
}
