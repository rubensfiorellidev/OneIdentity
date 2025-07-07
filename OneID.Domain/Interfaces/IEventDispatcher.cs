using OneID.Domain.Abstractions.EventsContext;

namespace OneID.Domain.Interfaces
{
    public interface IEventDispatcher
    {
        Task HandleAsync(IEnumerable<Event> domainEvents, CancellationToken cancellationToken);

    }
}
