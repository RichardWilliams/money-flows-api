namespace PropertyManagement.Api.Features.Expenses.Domain;

public sealed class ExpenseAttachment
{
    public Guid Id { get; init; }
    public Guid ExpenseId { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long SizeBytes { get; set; }
    public required string StoragePath { get; set; }
    public DateTime UploadedAt { get; init; }

    public static ExpenseAttachment Create(
        Guid expenseId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath)
    {
        return new ExpenseAttachment
        {
            Id = Guid.NewGuid(),
            ExpenseId = expenseId,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            StoragePath = storagePath,
            UploadedAt = DateTime.UtcNow
        };
    }
}
