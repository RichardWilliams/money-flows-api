namespace PropertyManagement.Api.Features.Tenants.Domain;

public sealed class Tenant
{
    public Guid Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public static Tenant Create(
        string firstName,
        string lastName,
        string email,
        string? phone = null,
        string? emergencyContact = null,
        string? emergencyPhone = null,
        string? notes = null)
    {
        return new Tenant
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email.ToLowerInvariant(),
            Phone = phone,
            EmergencyContact = emergencyContact,
            EmergencyPhone = emergencyPhone,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string firstName,
        string lastName,
        string email,
        string? phone,
        string? emergencyContact,
        string? emergencyPhone,
        string? notes)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email.ToLowerInvariant();
        Phone = phone;
        EmergencyContact = emergencyContact;
        EmergencyPhone = emergencyPhone;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
