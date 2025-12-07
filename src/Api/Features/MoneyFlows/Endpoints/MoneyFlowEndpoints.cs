using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.Api.Features.MoneyFlows.Application.Commands;
using PropertyManagement.Api.Features.MoneyFlows.Application.Dtos;
using PropertyManagement.Api.Features.MoneyFlows.Application.Queries;
using PropertyManagement.Api.Shared.Models;

namespace PropertyManagement.Api.Features.MoneyFlows.Endpoints;

public static class MoneyFlowEndpoints
{
    public static RouteGroupBuilder MapMoneyFlowEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/moneyflows")
            .WithTags("MoneyFlows")
            .WithOpenApi();

        group.MapPost("/", CreateMoneyFlow)
            .WithName("CreateMoneyFlow")
            .WithSummary("Create a new money flow (income or expense)")
            .Produces<MoneyFlowDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateMoneyFlow)
            .WithName("UpdateMoneyFlow")
            .WithSummary("Update an existing money flow")
            .Produces<MoneyFlowDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}", GetMoneyFlow)
            .WithName("GetMoneyFlow")
            .WithSummary("Get a money flow by ID")
            .Produces<MoneyFlowDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", ListMoneyFlows)
            .WithName("ListMoneyFlows")
            .WithSummary("List money flows with optional filtering")
            .Produces<PagedList<MoneyFlowListDto>>(StatusCodes.Status200OK);

        group.MapDelete("/{id:guid}", DeleteMoneyFlow)
            .WithName("DeleteMoneyFlow")
            .WithSummary("Delete a money flow")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> CreateMoneyFlow(
        [FromBody] CreateMoneyFlowRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateMoneyFlowCommand(
            request.PropertyId,
            request.Type,
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

        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/moneyflows/{result.Id}", result);
    }

    private static async Task<IResult> UpdateMoneyFlow(
        [FromRoute] Guid id,
        [FromBody] UpdateMoneyFlowRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMoneyFlowCommand(
            id,
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

        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetMoneyFlow(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetMoneyFlowQuery(id);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ListMoneyFlows(
        [FromServices] ISender sender,
        [FromQuery] Guid? propertyId = null,
        [FromQuery] int? type = null,
        [FromQuery] DateOnly? dateFrom = null,
        [FromQuery] DateOnly? dateTo = null,
        [FromQuery] Guid? expenseCategoryId = null,
        [FromQuery] Guid? tenantId = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new ListMoneyFlowsQuery(
            propertyId,
            type,
            dateFrom,
            dateTo,
            expenseCategoryId,
            tenantId,
            searchTerm,
            pageNumber,
            pageSize);

        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteMoneyFlow(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteMoneyFlowCommand(id);
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }
}

public sealed record CreateMoneyFlowRequest(
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
    string? Notes);

public sealed record UpdateMoneyFlowRequest(
    decimal Amount,
    string Currency,
    DateOnly Date,
    string Description,
    Guid? ExpenseCategoryId,
    string? IncomeSource,
    Guid? TenantId,
    Guid? LeaseId,
    string? Reference,
    string? Notes);
