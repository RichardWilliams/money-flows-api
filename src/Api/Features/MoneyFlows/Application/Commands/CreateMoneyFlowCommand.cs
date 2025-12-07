using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.MoneyFlows.Application.Dtos;
using PropertyManagement.Api.Features.MoneyFlows.Domain;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.MoneyFlows.Application.Commands;

public sealed record CreateMoneyFlowCommand(
    Guid PropertyId,
    int Type,
    decimal Amount,
    string Currency,
    DateOnly Date,
    string Description,
    Guid? ExpenseCategoryId,
    string? IncomeSource,
    Guid? TenantId,
    Guid? LeaseId,
    string? Reference,
    string? Notes) : IRequest<MoneyFlowDto>;

internal sealed class CreateMoneyFlowCommandValidator : AbstractValidator<CreateMoneyFlowCommand>
{
    public CreateMoneyFlowCommandValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty()
            .WithMessage("PropertyId is required");

        RuleFor(x => x.Type)
            .Must(t => t == 1 || t == 2)
            .WithMessage("Type must be 1 (Income) or 2 (Expense)");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Must(c => new[] { "GBP", "CHF", "EUR", "USD" }.Contains(c))
            .WithMessage("Currency must be one of: GBP, CHF, EUR, USD");

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date cannot be in the future");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(500)
            .WithMessage("Description must be between 3 and 500 characters");

        RuleFor(x => x.Reference)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Reference));

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

internal sealed class CreateMoneyFlowCommandHandler : IRequestHandler<CreateMoneyFlowCommand, MoneyFlowDto>
{
    private readonly PropertyManagementDbContext _context;

    public CreateMoneyFlowCommandHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<MoneyFlowDto> Handle(CreateMoneyFlowCommand request, CancellationToken cancellationToken)
    {
        // Validate property exists
        var propertyExists = await _context.Properties
            .AnyAsync(p => p.Id == request.PropertyId, cancellationToken);

        if (!propertyExists)
            throw new NotFoundException($"Property with ID {request.PropertyId} not found");

        // Validate expense category if provided
        string? expenseCategoryName = null;
        if (request.ExpenseCategoryId.HasValue)
        {
            var category = await _context.ExpenseCategories
                .FirstOrDefaultAsync(c => c.Id == request.ExpenseCategoryId.Value, cancellationToken);

            if (category == null)
                throw new NotFoundException($"Expense category with ID {request.ExpenseCategoryId} not found");

            expenseCategoryName = category.Name;
        }

        // Validate tenant if provided
        if (request.TenantId.HasValue)
        {
            var tenantExists = await _context.Tenants
                .AnyAsync(t => t.Id == request.TenantId.Value, cancellationToken);

            if (!tenantExists)
                throw new NotFoundException($"Tenant with ID {request.TenantId} not found");
        }

        // Validate lease if provided
        if (request.LeaseId.HasValue)
        {
            var leaseExists = await _context.Leases
                .AnyAsync(l => l.Id == request.LeaseId.Value, cancellationToken);

            if (!leaseExists)
                throw new NotFoundException($"Lease with ID {request.LeaseId} not found");
        }

        var moneyFlow = MoneyFlow.Create(
            request.PropertyId,
            (MoneyFlowType)request.Type,
            request.Amount,
            request.Currency,
            request.Date,
            request.Description,
            request.ExpenseCategoryId,
            request.IncomeSource,
            request.TenantId,
            request.LeaseId,
            request.Reference,
            request.Notes);

        _context.MoneyFlows.Add(moneyFlow);
        await _context.SaveChangesAsync(cancellationToken);

        return new MoneyFlowDto
        {
            Id = moneyFlow.Id,
            PropertyId = moneyFlow.PropertyId,
            Type = (int)moneyFlow.Type,
            TypeName = moneyFlow.Type.ToString(),
            Amount = moneyFlow.Amount,
            Currency = moneyFlow.Currency,
            Date = moneyFlow.Date,
            Description = moneyFlow.Description,
            ExpenseCategoryId = moneyFlow.ExpenseCategoryId,
            ExpenseCategoryName = expenseCategoryName,
            IncomeSource = moneyFlow.IncomeSource,
            TenantId = moneyFlow.TenantId,
            LeaseId = moneyFlow.LeaseId,
            Reference = moneyFlow.Reference,
            Notes = moneyFlow.Notes,
            CreatedAt = moneyFlow.CreatedAt,
            UpdatedAt = moneyFlow.UpdatedAt
        };
    }
}
