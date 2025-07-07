using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Repositories;
using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Contracts;
using OneID.Domain.EventsFromDomain;
using OneID.Domain.Helpers;

namespace OneID.Application.Services.StrategyEvents
{
    public sealed class UserAccountCreatedEventStrategy : IEventStrategy
    {
        private readonly IStoredEventRepository _repository;
        private readonly ILogger<UserAccountCreatedEventStrategy> _logger;

        public Type EventType => typeof(UserAccountCreatedEvent);

        public UserAccountCreatedEventStrategy(IStoredEventRepository repository, ILogger<UserAccountCreatedEventStrategy> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(Event domainEvent, CancellationToken cancellationToken)
        {
            if (domainEvent is not UserAccountCreatedEvent createdEvent)
            {
                _logger.LogWarning("Event type mismatch in {Strategy}. Received {EventType}", nameof(UserAccountCreatedEventStrategy), domainEvent.GetType().Name);
                return;
            }

            var storedEvent = new StoredEvent
            {
                Id = createdEvent.Id,
                AggregateId = createdEvent.AccountId,
                AggregateType = createdEvent.AggregateType,
                EventType = "Created",
                OccurredOn = createdEvent.OccurredOn,
                Version = createdEvent.Version,
                Description = createdEvent.Description,
                CreatedBy = createdEvent.CreatedBy,
                EventData = EventSerializer.SerializeToJson(createdEvent)
            };

            try
            {
                await _repository.AddAsync(storedEvent, cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("UserAccountCreatedEvent persisted for {AccountId}", createdEvent.AccountId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist UserAccountCreatedEvent for {AccountId}", createdEvent.AccountId);
                throw;
            }
        }
    }
}
