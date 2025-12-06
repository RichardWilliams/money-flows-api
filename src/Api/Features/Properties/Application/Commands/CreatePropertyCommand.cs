using FluentValidation;
using MediatR;
using PropertyManagement.Api.Features.Properties.Application.Dtos;
using PropertyManagement.Api.Features.Properties.Domain;
using PropertyManagement.Api.Infrastructure.Persistence;

namespace PropertyManagement.Api.Features.Properties.Application.Commands;

public sealed record CreatePropertyCommand(
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
    string? Description) : IRequest<PropertyDto>;

internal sealed class CreatePropertyCommandValidator : AbstractValidator<CreatePropertyCommand>
{
    public CreatePropertyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.County)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Postcode)
            .NotEmpty()
            .MaximumLength(10)
            .Matches(@"^[A-Z]{1,2}[0-9][A-Z0-9]? ?[0-9][A-Z]{2}$")
            .WithMessage("Must be a valid UK postcode");

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(20);

        RuleFor(x => x.Bathrooms)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(10);

        RuleFor(x => x.PurchasePrice)
            .GreaterThan(0)
            .When(x => x.PurchasePrice.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}

internal sealed class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, PropertyDto>
{
    private readonly PropertyManagementDbContext _context;

    public CreatePropertyCommandHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<PropertyDto> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = Property.Create(
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

        _context.Properties.Add(property);
        await _context.SaveChangesAsync(cancellationToken);

        return new PropertyDto
        {
            Id = property.Id,
            Name = property.Name,
            AddressLine1 = property.AddressLine1,
            AddressLine2 = property.AddressLine2,
            City = property.City,
            County = property.County,
            Postcode = property.Postcode,
            Type = property.Type,
            Status = property.Status,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            PurchasePrice = property.PurchasePrice,
            PurchaseDate = property.PurchaseDate,
            Description = property.Description,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };
    }
}
