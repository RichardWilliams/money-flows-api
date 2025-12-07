namespace PropertyManagement.Api.Features.Tenants.Domain;

public sealed class Lease
{
    public Guid Id { get; init; }
    public Guid PropertyId { get; set; }
    public Guid TenantId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public required string Currency { get; set; }
    public decimal? DepositAmount { get; set; }
    public int RentDayOfMonth { get; set; }
    public LeaseStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public static Lease Create(
        Guid propertyId,
        Guid tenantId,
        DateOnly startDate,
        DateOnly? endDate,
        decimal monthlyRent,
        string currency,
        decimal? depositAmount,
        int rentDayOfMonth,
        string? notes = null)
    {
        return new Lease
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            TenantId = tenantId,
            StartDate = startDate,
            EndDate = endDate,
            MonthlyRent = monthlyRent,
            Currency = currency.ToUpperInvariant(),
            DepositAmount = depositAmount,
            RentDayOfMonth = rentDayOfMonth,
            Status = LeaseStatus.Active,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        DateOnly startDate,
        DateOnly? endDate,
        decimal monthlyRent,
        string currency,
        decimal? depositAmount,
        int rentDayOfMonth,
        string? notes)
    {
        StartDate = startDate;
        EndDate = endDate;
        MonthlyRent = monthlyRent;
        Currency = currency.ToUpperInvariant();
        DepositAmount = depositAmount;
        RentDayOfMonth = rentDayOfMonth;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Terminate(DateOnly endDate)
    {
        EndDate = endDate;
        Status = LeaseStatus.Terminated;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Expire()
    {
        Status = LeaseStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
    }
}
