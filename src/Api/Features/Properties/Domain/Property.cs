namespace PropertyManagement.Api.Features.Properties.Domain;

public sealed class Property
{
    public Guid Id { get; init; }
    public required string Name { get; set; }
    public required string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public required string City { get; set; }
    public required string County { get; set; }
    public required string Postcode { get; set; }
    public required PropertyType Type { get; set; }
    public required PropertyStatus Status { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal? PurchasePrice { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public static Property Create(
        string name,
        string addressLine1,
        string? addressLine2,
        string city,
        string county,
        string postcode,
        PropertyType type,
        int bedrooms,
        int bathrooms,
        decimal? purchasePrice = null,
        DateOnly? purchaseDate = null,
        string? description = null)
    {
        return new Property
        {
            Id = Guid.NewGuid(),
            Name = name,
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            City = city,
            County = county,
            Postcode = postcode.ToUpperInvariant().Replace(" ", ""),
            Type = type,
            Status = PropertyStatus.Active,
            Bedrooms = bedrooms,
            Bathrooms = bathrooms,
            PurchasePrice = purchasePrice,
            PurchaseDate = purchaseDate,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        string addressLine1,
        string? addressLine2,
        string city,
        string county,
        string postcode,
        PropertyType type,
        int bedrooms,
        int bathrooms,
        decimal? purchasePrice,
        DateOnly? purchaseDate,
        string? description)
    {
        Name = name;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        County = county;
        Postcode = postcode.ToUpperInvariant().Replace(" ", "");
        Type = type;
        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        PurchasePrice = purchasePrice;
        PurchaseDate = purchaseDate;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        Status = PropertyStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = PropertyStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }
}
