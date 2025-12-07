namespace PropertyManagement.Api.Features.Reports.Application.Dtos;

public sealed record PropertySummaryDto
{
    public required Guid PropertyId { get; init; }
    public required string PropertyName { get; init; }
    public required ReportPeriodDto ReportPeriod { get; init; }
    public required string Currency { get; init; }
    public required IncomeBreakdownDto Income { get; init; }
    public required ReportExpenseBreakdownDto Expenses { get; init; }
    public decimal NetIncome { get; init; }
    public decimal ProfitMargin { get; init; }
}

public sealed record ReportPeriodDto
{
    public required DateOnly From { get; init; }
    public required DateOnly To { get; init; }
}

public sealed record IncomeBreakdownDto
{
    public decimal TotalAmount { get; init; }
    public int TransactionCount { get; init; }
    public required List<IncomeSourceBreakdownDto> Breakdown { get; init; }
}

public sealed record IncomeSourceBreakdownDto
{
    public required string Source { get; init; }
    public decimal Amount { get; init; }
    public int Count { get; init; }
}

public sealed record ReportExpenseBreakdownDto
{
    public decimal TotalAmount { get; init; }
    public int TransactionCount { get; init; }
    public required List<ExpenseCategoryBreakdownDto> Breakdown { get; init; }
}

public sealed record ExpenseCategoryBreakdownDto
{
    public required Guid CategoryId { get; init; }
    public required string CategoryName { get; init; }
    public decimal Amount { get; init; }
    public int Count { get; init; }
}
