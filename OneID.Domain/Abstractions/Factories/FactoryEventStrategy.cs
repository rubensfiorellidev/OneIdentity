using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Contracts;

#nullable disable
namespace OneID.Domain.Abstractions.Factories
{
    public sealed class FactoryEventStrategy : IFactoryEventStrategy
    {
        private readonly Dictionary<Type, IEventStrategy> _strategies;

        public FactoryEventStrategy(IEnumerable<IEventStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(s => s.EventType, s => s);
        }

        public IEventStrategy GetStrategy(Event domainEvent)
        {
            ArgumentNullException.ThrowIfNull(domainEvent);

            _strategies.TryGetValue(domainEvent.GetType(), out var strategy);
            return strategy;
        }
    }
}
