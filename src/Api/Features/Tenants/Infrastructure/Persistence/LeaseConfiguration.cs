using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Api.Features.Tenants.Domain;

namespace PropertyManagement.Api.Features.Tenants.Infrastructure.Persistence;

internal sealed class LeaseConfiguration : IEntityTypeConfiguration<Lease>
{
    public void Configure(EntityTypeBuilder<Lease> builder)
    {
        builder.ToTable("leases");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedNever();

        builder.Property(l => l.PropertyId)
            .IsRequired();

        builder.Property(l => l.TenantId)
            .IsRequired();

        builder.Property(l => l.StartDate)
            .IsRequired();

        builder.Property(l => l.EndDate);

        builder.Property(l => l.MonthlyRent)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(l => l.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(l => l.DepositAmount)
            .HasPrecision(18, 2);

        builder.Property(l => l.RentDayOfMonth)
            .IsRequired();

        builder.Property(l => l.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(l => l.Notes)
            .HasMaxLength(2000);

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .IsRequired();

        builder.HasIndex(l => l.PropertyId);
        builder.HasIndex(l => l.TenantId);
        builder.HasIndex(l => l.Status);
        builder.HasIndex(l => new { l.PropertyId, l.Status });
    }
}
