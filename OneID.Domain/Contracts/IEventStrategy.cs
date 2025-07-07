using OneID.Domain.Abstractions.EventsContext;

namespace OneID.Domain.Contracts
{
    public interface IEventStrategy
    {
        Type EventType { get; }
        Task HandleAsync(Event domainEvent, CancellationToken cancellationToken);

    }
}
