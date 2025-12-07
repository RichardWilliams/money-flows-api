using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Api.Features.Expenses.Domain;

namespace PropertyManagement.Api.Features.Expenses.Infrastructure.Persistence;

internal sealed class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.ToTable("expense_categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        builder.HasIndex(c => c.Code)
            .IsUnique();

        builder.HasIndex(c => c.IsActive);

        // Seed default expense categories
        var now = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Maintenance & Repairs", Code = "MAINTENANCE", Description = "Property maintenance and repair costs", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-111111111112"), Name = "Utilities", Code = "UTILITIES", Description = "Gas, electricity, water, and other utilities", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-111111111113"), Name = "Insurance", Code = "INSURANCE", Description = "Property and landlord insurance", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-111111111114"), Name = "Council Tax", Code = "COUNCIL_TAX", Description = "Council tax payments", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-111111111115"), Name = "Mortgage Interest", Code = "MORTGAGE", Description = "Mortgage interest payments", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-111111111116"), Name = "Service Charges", Code = "SERVICE_CHARGE", Description = "Building or management service charges", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-111111111117"), Name = "Ground Rent", Code = "GROUND_RENT", Description = "Ground rent payments", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-111111111118"), Name = "Letting Agent Fees", Code = "AGENT_FEES", Description = "Letting agent fees and commissions", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-111111111119"), Name = "Legal & Professional", Code = "LEGAL", Description = "Legal and professional fees", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-11111111111a"), Name = "Furnishings", Code = "FURNISHINGS", Description = "Furniture and white goods", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-11111111111b"), Name = "Gardening", Code = "GARDENING", Description = "Garden maintenance and landscaping", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-11111111111c"), Name = "Advertising", Code = "ADVERTISING", Description = "Property advertising costs", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new ExpenseCategory { Id = Guid.Parse("11111111-1111-1111-1111-11111111111d"), Name = "Other", Code = "OTHER", Description = "Miscellaneous expenses", IsActive = true, CreatedAt = now, UpdatedAt = now }
        );
    }
}
