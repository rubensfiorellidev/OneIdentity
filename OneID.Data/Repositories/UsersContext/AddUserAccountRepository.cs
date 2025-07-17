using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.UserContext;
using OneID.Domain.Interfaces;
using OneID.Domain.Results;

#nullable disable
namespace OneID.Data.Repositories.UsersContext
{
    public class AddUserAccountRepository : IAddUserAccountRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;
        private readonly ILogger<AddUserAccountRepository> _logger;

        public AddUserAccountRepository(IOneDbContextFactory dbContextFactory,
                                        ILogger<AddUserAccountRepository> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task<IResult> AddAsync(UserAccount entity, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            var strategy = dbContext.Database.CreateExecutionStrategy();

            try
            {
                return await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                    try
                    {
                        await dbContext.UserAccounts.AddAsync(entity, cancellationToken);

                        var rows = await dbContext.SaveChangesAsync(cancellationToken);

                        await transaction.CommitAsync(cancellationToken);

                        _logger.LogInformation("UserAccount {Id} persisted successfully with {Rows} rows affected.", entity.Id, rows);

                        return Result.Success($"UserAccount {entity.Id} persisted successfully.", entity);
                    }
                    catch (DbUpdateException dbEx)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        _logger.LogError(dbEx, "Database update exception while persisting UserAccount {Id}.", entity.Id);
                        return Result.Failure("Database update error occurred while persisting the UserAccount.");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        _logger.LogError(ex, "Unexpected error while persisting UserAccount {Id}.", entity.Id);
                        return Result.Failure("Unexpected error occurred while persisting the UserAccount.");
                    }
                });
            }
            catch (Exception outerEx)
            {
                _logger.LogError(outerEx, "Execution strategy failed for UserAccount {Id}.", entity.Id);
                return Result.Failure("Execution strategy failure while persisting the UserAccount.");
            }
        }
    }
}
