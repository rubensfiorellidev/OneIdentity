using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Repositories;
using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Contracts;
using OneID.Domain.EventsFromDomain;
using OneID.Domain.Helpers;

namespace OneID.Application.Services.StrategyEvents
{
    public sealed class UserAccountCreationFailedEventStrategy : IEventStrategy
    {
        private readonly IStoredEventRepository _repository;
        private readonly ILogger<UserAccountCreationFailedEventStrategy> _logger;

        public Type EventType => typeof(UserAccountCreationFailedEvent);

        public UserAccountCreationFailedEventStrategy(IStoredEventRepository repository, ILogger<UserAccountCreationFailedEventStrategy> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(Event domainEvent, CancellationToken cancellationToken)
        {
            if (domainEvent is not UserAccountCreationFailedEvent failedEvent)
            {
                _logger.LogWarning("Event type mismatch in {Strategy}. Received {EventType}", nameof(UserAccountCreationFailedEventStrategy), domainEvent.GetType().Name);
                return;
            }

            var storedEvent = new StoredEvent
            {
                Id = failedEvent.Id,
                AggregateId = failedEvent.AccountId,
                AggregateType = failedEvent.AggregateType,
                EventType = "Failed",
                OccurredOn = failedEvent.OccurredOn,
                Version = failedEvent.Version,
                Description = failedEvent.Description,
                CreatedBy = failedEvent.CreatedBy,
                EventData = EventSerializer.SerializeToJson(failedEvent)
            };

            try
            {
                await _repository.AddAsync(storedEvent, cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("UserAccountCreationFailedEvent persisted for {AccountId}", failedEvent.AccountId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist UserAccountCreationFailedEvent for {AccountId}", failedEvent.AccountId);
                throw;
            }
        }
    }

}
