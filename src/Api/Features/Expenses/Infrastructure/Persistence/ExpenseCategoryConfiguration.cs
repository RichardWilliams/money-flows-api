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
        var now = DateTime.UtcNow;
        builder.HasData(
            ExpenseCategory.Create("Maintenance & Repairs", "MAINTENANCE", "Property maintenance and repair costs") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Utilities", "UTILITIES", "Gas, electricity, water, and other utilities") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Insurance", "INSURANCE", "Property and landlord insurance") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Council Tax", "COUNCIL_TAX", "Council tax payments") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Mortgage Interest", "MORTGAGE", "Mortgage interest payments") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Service Charges", "SERVICE_CHARGE", "Building or management service charges") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Ground Rent", "GROUND_RENT", "Ground rent payments") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Letting Agent Fees", "AGENT_FEES", "Letting agent fees and commissions") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Legal & Professional", "LEGAL", "Legal and professional fees") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Furnishings", "FURNISHINGS", "Furniture and white goods") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Gardening", "GARDENING", "Garden maintenance and landscaping") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Advertising", "ADVERTISING", "Property advertising costs") with { CreatedAt = now, UpdatedAt = now },
            ExpenseCategory.Create("Other", "OTHER", "Miscellaneous expenses") with { CreatedAt = now, UpdatedAt = now }
        );
    }
}
