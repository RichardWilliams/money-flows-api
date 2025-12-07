using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.Api.Features.Properties.Application.Commands;
using PropertyManagement.Api.Features.Properties.Application.Dtos;
using PropertyManagement.Api.Features.Properties.Application.Queries;
using PropertyManagement.Api.Features.Properties.Domain;
using PropertyManagement.Api.Shared.Models;

namespace PropertyManagement.Api.Features.Properties.Endpoints;

public static class PropertyEndpoints
{
    public static RouteGroupBuilder MapPropertyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/properties")
            .WithTags("Properties")
            .WithOpenApi();

        group.MapPost("/", CreateProperty)
            .WithName("CreateProperty")
            .WithSummary("Create a new property")
            .Produces<PropertyDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateProperty)
            .WithName("UpdateProperty")
            .WithSummary("Update an existing property")
            .Produces<PropertyDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}", GetProperty)
            .WithName("GetProperty")
            .WithSummary("Get a property by ID")
            .Produces<PropertyDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", ListProperties)
            .WithName("ListProperties")
            .WithSummary("List all properties with optional filtering")
            .Produces<PagedList<PropertyListDto>>(StatusCodes.Status200OK);

        group.MapDelete("/{id:guid}", ArchiveProperty)
            .WithName("ArchiveProperty")
            .WithSummary("Archive a property")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> CreateProperty(
        [FromBody] CreatePropertyRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreatePropertyCommand(
            request.Name,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.County,
            request.Postcode,
            request.Type,
            request.Bedrooms,
            request.Bathrooms,
            request.PurchasePrice,
            request.PurchaseDate,
            request.Description);

        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/properties/{result.Id}", result);
    }

    private static async Task<IResult> UpdateProperty(
        [FromRoute] Guid id,
        [FromBody] UpdatePropertyRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePropertyCommand(
            id,
            request.Name,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.County,
            request.Postcode,
            request.Type,
            request.Bedrooms,
            request.Bathrooms,
            request.PurchasePrice,
            request.PurchaseDate,
            request.Description);

        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProperty(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetPropertyQuery(id);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ListProperties(
        [FromServices] ISender sender,
        [FromQuery] PropertyStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new ListPropertiesQuery(status, pageNumber, pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ArchiveProperty(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ArchivePropertyCommand(id);
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }
}

public sealed record CreatePropertyRequest(
    string Name,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string County,
    string Postcode,
    PropertyType Type,
    int Bedrooms,
    int Bathrooms,
    decimal? PurchasePrice,
    DateOnly? PurchaseDate,
    string? Description);

public sealed record UpdatePropertyRequest(
    string Name,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string County,
    string Postcode,
    PropertyType Type,
    int Bedrooms,
    int Bathrooms,
    decimal? PurchasePrice,
    DateOnly? PurchaseDate,
    string? Description);
