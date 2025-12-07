using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Api.Features.Expenses.Domain;

namespace PropertyManagement.Api.Features.Expenses.Infrastructure.Persistence;

internal sealed class ExpenseAttachmentConfiguration : IEntityTypeConfiguration<ExpenseAttachment>
{
    public void Configure(EntityTypeBuilder<ExpenseAttachment> builder)
    {
        builder.ToTable("expense_attachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.ExpenseId)
            .IsRequired();

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.SizeBytes)
            .IsRequired();

        builder.Property(a => a.StoragePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.UploadedAt)
            .IsRequired();

        builder.HasIndex(a => a.ExpenseId);
    }
}
