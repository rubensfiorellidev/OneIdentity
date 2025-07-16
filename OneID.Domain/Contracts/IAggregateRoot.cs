using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Notifications;

namespace OneID.Domain.Contracts
{
    public interface IAggregateRoot
    {
        string Id { get; }

        IReadOnlyCollection<Event> Events { get; }
        IReadOnlyCollection<Notification> Notifications { get; }

        void AddEvent(Event domainEvent);
        void ClearEvents();
        void AddNotification(Notification notification);
        void AddNotifications(IEnumerable<Notification> notifications);
    }

}
