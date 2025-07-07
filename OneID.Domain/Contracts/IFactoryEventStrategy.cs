using OneID.Domain.Abstractions.EventsContext;

namespace OneID.Domain.Contracts
{
    public interface IFactoryEventStrategy
    {
        IEventStrategy GetStrategy(Event domainEvent);
    }
}
