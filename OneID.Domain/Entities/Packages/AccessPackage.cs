using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Contracts;
using OneID.Domain.Notifications;
#nullable disable
namespace OneID.Domain.Entities.Packages
{
    public class AccessPackage : IAggregateRoot
    {
        private readonly List<AccessPackageItem> _items = [];
        private readonly List<Notification> _notifications = [];
        private readonly List<Event> _events = [];

        public AccessPackage(string name, string createdBy)
        {
            Name = name;
            CreatedBy = createdBy;
            UpdatedBy = createdBy;
            ProvisioningAt = DateTimeOffset.UtcNow;
            _items = [];
        }

        public string Id { get; private set; }
        public string Name { get; private set; }

        public string CreatedBy { get; private set; }
        public string UpdatedBy { get; private set; }
        public DateTimeOffset ProvisioningAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }

        public IReadOnlyCollection<AccessPackageItem> Items => _items.AsReadOnly();
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();
        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();

        public void AddItem(string type, string value)
        {
            var item = new AccessPackageItem(Id, type, value);
            _items.Add(item);
        }

        public void SetUpdatedBy(string updatedBy)
        {
            UpdatedBy = updatedBy;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void AddEvent(Event domainEvent) => _events.Add(domainEvent);
        public void ClearEvents() => _events.Clear();

        public void AddNotification(Notification notification) => _notifications.Add(notification);
        public void AddNotifications(IEnumerable<Notification> notifications) => _notifications.AddRange(notifications);

        public void PrepareForUpdate(string updatedBy) => SetUpdatedBy(updatedBy);

        public bool IsValid() => true;
    }

}
