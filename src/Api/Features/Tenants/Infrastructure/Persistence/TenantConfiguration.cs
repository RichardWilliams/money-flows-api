using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Api.Features.Tenants.Domain;

namespace PropertyManagement.Api.Features.Tenants.Infrastructure.Persistence;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.Phone)
            .HasMaxLength(20);

        builder.Property(t => t.EmergencyContact)
            .HasMaxLength(200);

        builder.Property(t => t.EmergencyPhone)
            .HasMaxLength(20);

        builder.Property(t => t.Notes)
            .HasMaxLength(2000);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        builder.HasIndex(t => t.Email);
        builder.HasIndex(t => t.LastName);
    }
}
