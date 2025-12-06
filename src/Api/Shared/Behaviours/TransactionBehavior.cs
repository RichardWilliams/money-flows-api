using System.Data;
using MediatR;
using PropertyManagement.Api.Infrastructure.Persistence;

namespace PropertyManagement.Api.Shared.Behaviours;

internal sealed class TransactionBehavior<TRequest, TResponse>(
    PropertyManagementDbContext dbContext,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only wrap commands (not queries) in transactions
        if (typeof(TRequest).Name.EndsWith("Query"))
        {
            return await next();
        }

        logger.LogDebug("Beginning transaction for {RequestName}", typeof(TRequest).Name);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        try
        {
            var response = await next();

            await transaction.CommitAsync(cancellationToken);
            logger.LogDebug("Transaction committed for {RequestName}", typeof(TRequest).Name);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Transaction rollback for {RequestName}",
                typeof(TRequest).Name);

            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
