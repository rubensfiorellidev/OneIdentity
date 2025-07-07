using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Contracts.Validations;
using OneID.Domain.Notifications;

namespace OneID.Domain.Contracts
{
    public interface IAggregateRoot : IValidation, IContract
    {
        string Id { get; }
        DateTimeOffset ProvisioningAt { get; }
        DateTimeOffset? UpdatedAt { get; }
        string CreatedBy { get; }
        string UpdatedBy { get; }

        IReadOnlyCollection<Event> Events { get; }
        IReadOnlyCollection<Notification> Notifications { get; }

        void AddEvent(Event domainEvent);
        void ClearEvents();
        void AddNotification(Notification notification);
        void AddNotifications(IEnumerable<Notification> notifications);
        void PrepareForUpdate(string updatedBy);
    }

}
