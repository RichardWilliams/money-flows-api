using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Api.Features.MoneyFlows.Domain;

namespace PropertyManagement.Api.Features.MoneyFlows.Infrastructure.Persistence;

internal sealed class MoneyFlowConfiguration : IEntityTypeConfiguration<MoneyFlow>
{
    public void Configure(EntityTypeBuilder<MoneyFlow> builder)
    {
        builder.ToTable("money_flows", "property_management");

        builder.HasKey(mf => mf.Id);

        builder.Property(mf => mf.Id)
            .ValueGeneratedNever();

        builder.Property(mf => mf.PropertyId)
            .IsRequired();

        builder.Property(mf => mf.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(mf => mf.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(mf => mf.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(mf => mf.Date)
            .IsRequired();

        builder.Property(mf => mf.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(mf => mf.IncomeSource)
            .HasMaxLength(255);

        builder.Property(mf => mf.Reference)
            .HasMaxLength(100);

        builder.Property(mf => mf.Notes)
            .HasColumnType("text");

        builder.Property(mf => mf.CreatedAt)
            .IsRequired();

        builder.Property(mf => mf.UpdatedAt)
            .IsRequired();

        // Indexes as per schema requirements
        builder.HasIndex(mf => new { mf.PropertyId, mf.Date })
            .HasDatabaseName("idx_money_flows_property_date");

        builder.HasIndex(mf => mf.Type)
            .HasDatabaseName("idx_money_flows_type");

        builder.HasIndex(mf => mf.Date)
            .HasDatabaseName("idx_money_flows_date");

        builder.HasIndex(mf => mf.ExpenseCategoryId)
            .HasDatabaseName("idx_money_flows_expense_category");

        builder.HasIndex(mf => mf.TenantId)
            .HasDatabaseName("idx_money_flows_tenant");
    }
}
