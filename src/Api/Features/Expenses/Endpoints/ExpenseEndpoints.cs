using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.Api.Features.Expenses.Application.Commands;
using PropertyManagement.Api.Features.Expenses.Application.Dtos;
using PropertyManagement.Api.Features.Expenses.Application.Queries;
using PropertyManagement.Api.Shared.Models;

namespace PropertyManagement.Api.Features.Expenses.Endpoints;

public static class ExpenseEndpoints
{
    public static RouteGroupBuilder MapExpenseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/expenses")
            .WithTags("Expenses")
            .WithOpenApi();

        group.MapPost("/", CreateExpense)
            .WithName("CreateExpense")
            .WithSummary("Create a new expense")
            .Produces<ExpenseDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateExpense)
            .WithName("UpdateExpense")
            .WithSummary("Update an existing expense")
            .Produces<ExpenseDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}", GetExpense)
            .WithName("GetExpense")
            .WithSummary("Get an expense by ID")
            .Produces<ExpenseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", ListExpenses)
            .WithName("ListExpenses")
            .WithSummary("List expenses with optional filtering")
            .Produces<PagedList<ExpenseListDto>>(StatusCodes.Status200OK);

        group.MapDelete("/{id:guid}", DeleteExpense)
            .WithName("DeleteExpense")
            .WithSummary("Delete an expense")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        var categoriesGroup = app.MapGroup("/api/expense-categories")
            .WithTags("Expense Categories")
            .WithOpenApi();

        categoriesGroup.MapGet("/", ListExpenseCategories)
            .WithName("ListExpenseCategories")
            .WithSummary("List all expense categories")
            .Produces<List<ExpenseCategoryDto>>(StatusCodes.Status200OK);

        return group;
    }

    private static async Task<IResult> CreateExpense(
        [FromBody] CreateExpenseRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateExpenseCommand(
            request.PropertyId,
            request.CategoryId,
            request.Description,
            request.Amount,
            request.Currency,
            request.Date,
            request.Vendor,
            request.Reference,
            request.Notes);

        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/expenses/{result.Id}", result);
    }

    private static async Task<IResult> UpdateExpense(
        [FromRoute] Guid id,
        [FromBody] UpdateExpenseRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateExpenseCommand(
            id,
            request.CategoryId,
            request.Description,
            request.Amount,
            request.Currency,
            request.Date,
            request.Vendor,
            request.Reference,
            request.Notes);

        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetExpense(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetExpenseQuery(id);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ListExpenses(
        [FromQuery] Guid? propertyId,
        [FromQuery] Guid? categoryId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromServices] ISender sender = default!,
        CancellationToken cancellationToken = default)
    {
        var query = new ListExpensesQuery(propertyId, categoryId, fromDate, toDate, pageNumber, pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteExpense(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteExpenseCommand(id);
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ListExpenseCategories(
        [FromQuery] bool? isActive,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new ListExpenseCategoriesQuery(isActive);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }
}

public sealed record CreateExpenseRequest(
    Guid PropertyId,
    Guid CategoryId,
    string Description,
    decimal Amount,
    string Currency,
    DateOnly Date,
    string? Vendor,
    string? Reference,
    string? Notes);

public sealed record UpdateExpenseRequest(
    Guid CategoryId,
    string Description,
    decimal Amount,
    string Currency,
    DateOnly Date,
    string? Vendor,
    string? Reference,
    string? Notes);
