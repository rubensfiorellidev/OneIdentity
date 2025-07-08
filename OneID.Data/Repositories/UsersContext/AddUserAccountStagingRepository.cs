using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Repositories.UsersContext
{
    public sealed class AddUserAccountStagingRepository : IAddUserAccountStagingRepository
    {
        private readonly IOneDbContextFactory _contextFactory;
        private readonly ILogger<AddUserAccountStagingRepository> _logger;
        public AddUserAccountStagingRepository(IOneDbContextFactory contextFactory,
                                               ILogger<AddUserAccountStagingRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task SaveAsync(AccountPjAdmissionStaging entity, CancellationToken cancellationToken)
        {
            try
            {
                await using var dbContext = _contextFactory.CreateDbContext();

                dbContext.Set<AccountPjAdmissionStaging>().Add(entity);

                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Staging salvo com sucesso: {CorrelationId}", entity.CorrelationId);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao persistir AccountPjAdmissionStaging: {CorrelationId}", entity.CorrelationId);

                throw;
            }
        }
    }
}
