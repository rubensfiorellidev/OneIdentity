using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Abstractions.EventsContext;

namespace OneID.Data.Repositories.StoredEvents
{
    public sealed class StoredEventRepository : IStoredEventRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;
        private readonly ILogger<StoredEventRepository> _logger;

        public StoredEventRepository(IOneDbContextFactory dbContextFactory, ILogger<StoredEventRepository> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task AddAsync(StoredEvent storedEvent, CancellationToken cancellationToken)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            ArgumentNullException.ThrowIfNull(storedEvent);

            try
            {
                await dbContext.StoredEvents.AddAsync(storedEvent, cancellationToken).ConfigureAwait(false);
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                _logger.LogInformation("StoredEvent {EventId} persisted successfully.", storedEvent.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DbUpdateException while persisting StoredEvent {EventId}", storedEvent.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while persisting StoredEvent {EventId}", storedEvent.Id);
                throw;
            }
        }
    }
}
