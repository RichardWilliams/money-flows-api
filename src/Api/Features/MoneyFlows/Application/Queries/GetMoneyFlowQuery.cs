using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.MoneyFlows.Application.Dtos;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.MoneyFlows.Application.Queries;

public sealed record GetMoneyFlowQuery(Guid Id) : IRequest<MoneyFlowDto>;

internal sealed class GetMoneyFlowQueryValidator : AbstractValidator<GetMoneyFlowQuery>
{
    public GetMoneyFlowQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

internal sealed class GetMoneyFlowQueryHandler : IRequestHandler<GetMoneyFlowQuery, MoneyFlowDto>
{
    private readonly PropertyManagementDbContext _context;

    public GetMoneyFlowQueryHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<MoneyFlowDto> Handle(GetMoneyFlowQuery request, CancellationToken cancellationToken)
    {
        var moneyFlow = await _context.MoneyFlows
            .AsNoTracking()
            .Where(mf => mf.Id == request.Id)
            .Select(mf => new
            {
                MoneyFlow = mf,
                CategoryName = mf.ExpenseCategoryId != null
                    ? _context.ExpenseCategories
                        .Where(c => c.Id == mf.ExpenseCategoryId.Value)
                        .Select(c => c.Name)
                        .FirstOrDefault()
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"MoneyFlow with ID {request.Id} not found");

        return new MoneyFlowDto
        {
            Id = moneyFlow.MoneyFlow.Id,
            PropertyId = moneyFlow.MoneyFlow.PropertyId,
            Type = (int)moneyFlow.MoneyFlow.Type,
            TypeName = moneyFlow.MoneyFlow.Type.ToString(),
            Amount = moneyFlow.MoneyFlow.Amount,
            Currency = moneyFlow.MoneyFlow.Currency,
            Date = moneyFlow.MoneyFlow.Date,
            Description = moneyFlow.MoneyFlow.Description,
            ExpenseCategoryId = moneyFlow.MoneyFlow.ExpenseCategoryId,
            ExpenseCategoryName = moneyFlow.CategoryName,
            IncomeSource = moneyFlow.MoneyFlow.IncomeSource,
            TenantId = moneyFlow.MoneyFlow.TenantId,
            LeaseId = moneyFlow.MoneyFlow.LeaseId,
            Reference = moneyFlow.MoneyFlow.Reference,
            Notes = moneyFlow.MoneyFlow.Notes,
            CreatedAt = moneyFlow.MoneyFlow.CreatedAt,
            UpdatedAt = moneyFlow.MoneyFlow.UpdatedAt
        };
    }
}
