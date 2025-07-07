using Microsoft.Extensions.Logging;
using OneID.Domain.Contracts;
using OneID.Domain.Interfaces;

namespace OneID.Domain.Abstractions.EventsContext
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IFactoryEventStrategy _eventStrategyFactory;
        private readonly ILogger<EventDispatcher> _logger;

        public EventDispatcher(IFactoryEventStrategy eventStrategyFactory, ILogger<EventDispatcher> logger)
        {
            _eventStrategyFactory = eventStrategyFactory ?? throw new ArgumentNullException(nameof(eventStrategyFactory));
            _logger = logger;
        }

        public async Task HandleAsync(IEnumerable<Event> domainEvents, CancellationToken cancellationToken)
        {
            if (domainEvents == null || !domainEvents.Any()) return;

            foreach (var domainEvent in domainEvents)
            {
                try
                {
                    var strategy = _eventStrategyFactory.GetStrategy(domainEvent);
                    if (strategy != null)
                    {
                        await strategy.HandleAsync(domainEvent, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        _logger.LogWarning("No strategy found for event type {EventType}", domainEvent.GetType().Name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing domain event {EventType}", domainEvent.GetType().Name);
                    throw;
                }
            }
        }
    }
}
