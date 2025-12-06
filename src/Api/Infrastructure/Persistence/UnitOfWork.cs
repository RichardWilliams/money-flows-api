namespace PropertyManagement.Api.Infrastructure.Persistence;

internal sealed class UnitOfWork(PropertyManagementDbContext dbContext) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
