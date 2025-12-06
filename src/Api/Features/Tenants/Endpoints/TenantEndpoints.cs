using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.Api.Features.Tenants.Application.Commands;
using PropertyManagement.Api.Features.Tenants.Application.Dtos;
using PropertyManagement.Api.Features.Tenants.Application.Queries;
using PropertyManagement.Api.Features.Tenants.Domain;
using PropertyManagement.Api.Shared.Models;

namespace PropertyManagement.Api.Features.Tenants.Endpoints;

public static class TenantEndpoints
{
    public static RouteGroupBuilder MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        var tenantsGroup = app.MapGroup("/api/tenants")
            .WithTags("Tenants")
            .WithOpenApi();

        tenantsGroup.MapPost("/", CreateTenant)
            .WithName("CreateTenant")
            .WithSummary("Create a new tenant")
            .Produces<TenantDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        tenantsGroup.MapPut("/{id:guid}", UpdateTenant)
            .WithName("UpdateTenant")
            .WithSummary("Update an existing tenant")
            .Produces<TenantDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

        tenantsGroup.MapGet("/{id:guid}", GetTenant)
            .WithName("GetTenant")
            .WithSummary("Get a tenant by ID")
            .Produces<TenantDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        tenantsGroup.MapGet("/", ListTenants)
            .WithName("ListTenants")
            .WithSummary("List all tenants with optional search")
            .Produces<PagedList<TenantDto>>(StatusCodes.Status200OK);

        var leasesGroup = app.MapGroup("/api/leases")
            .WithTags("Leases")
            .WithOpenApi();

        leasesGroup.MapPost("/", CreateLease)
            .WithName("CreateLease")
            .WithSummary("Create a new lease")
            .Produces<LeaseDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        leasesGroup.MapPut("/{id:guid}", UpdateLease)
            .WithName("UpdateLease")
            .WithSummary("Update an existing lease")
            .Produces<LeaseDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

        leasesGroup.MapPost("/{id:guid}/terminate", TerminateLease)
            .WithName("TerminateLease")
            .WithSummary("Terminate a lease")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        leasesGroup.MapGet("/{id:guid}", GetLease)
            .WithName("GetLease")
            .WithSummary("Get a lease by ID")
            .Produces<LeaseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        leasesGroup.MapGet("/", ListLeases)
            .WithName("ListLeases")
            .WithSummary("List leases with optional filtering")
            .Produces<PagedList<LeaseDto>>(StatusCodes.Status200OK);

        return tenantsGroup;
    }

    private static async Task<IResult> CreateTenant(
        [FromBody] CreateTenantRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateTenantCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.EmergencyContact,
            request.EmergencyPhone,
            request.Notes);

        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/tenants/{result.Id}", result);
    }

    private static async Task<IResult> UpdateTenant(
        [FromRoute] Guid id,
        [FromBody] UpdateTenantRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTenantCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.EmergencyContact,
            request.EmergencyPhone,
            request.Notes);

        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTenant(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetTenantQuery(id);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ListTenants(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromServices] ISender sender = default!,
        CancellationToken cancellationToken = default)
    {
        var query = new ListTenantsQuery(searchTerm, pageNumber, pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateLease(
        [FromBody] CreateLeaseRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateLeaseCommand(
            request.PropertyId,
            request.TenantId,
            request.StartDate,
            request.EndDate,
            request.MonthlyRent,
            request.Currency,
            request.DepositAmount,
            request.RentDayOfMonth,
            request.Notes);

        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/leases/{result.Id}", result);
    }

    private static async Task<IResult> UpdateLease(
        [FromRoute] Guid id,
        [FromBody] UpdateLeaseRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLeaseCommand(
            id,
            request.StartDate,
            request.EndDate,
            request.MonthlyRent,
            request.Currency,
            request.DepositAmount,
            request.RentDayOfMonth,
            request.Notes);

        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> TerminateLease(
        [FromRoute] Guid id,
        [FromBody] TerminateLeaseRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new TerminateLeaseCommand(id, request.EndDate);
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> GetLease(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetLeaseQuery(id);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ListLeases(
        [FromQuery] Guid? propertyId,
        [FromQuery] Guid? tenantId,
        [FromQuery] LeaseStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromServices] ISender sender = default!,
        CancellationToken cancellationToken = default)
    {
        var query = new ListLeasesQuery(propertyId, tenantId, status, pageNumber, pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }
}

public sealed record CreateTenantRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? EmergencyContact,
    string? EmergencyPhone,
    string? Notes);

public sealed record UpdateTenantRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? EmergencyContact,
    string? EmergencyPhone,
    string? Notes);

public sealed record CreateLeaseRequest(
    Guid PropertyId,
    Guid TenantId,
    DateOnly StartDate,
    DateOnly? EndDate,
    decimal MonthlyRent,
    string Currency,
    decimal? DepositAmount,
    int RentDayOfMonth,
    string? Notes);

public sealed record UpdateLeaseRequest(
    DateOnly StartDate,
    DateOnly? EndDate,
    decimal MonthlyRent,
    string Currency,
    decimal? DepositAmount,
    int RentDayOfMonth,
    string? Notes);

public sealed record TerminateLeaseRequest(DateOnly EndDate);
