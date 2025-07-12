using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Contracts;
using OneID.Domain.EventsFromDomain;
using OneID.Domain.Notifications;
using OneID.Domain.ValueObjects;

#nullable disable
namespace OneID.Domain.Entities.DepartmentContext
{
    public sealed class Department : IAggregateRoot, IAuditableEntity
    {
        private readonly List<Event> _events = [];
        private readonly List<Notification> _notifications = [];

        public string Id { get; private set; }
        public string Name { get; private set; }
        public DepartmentStatus Status { get; private set; }

        public DateTimeOffset ProvisioningAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public string CreatedBy { get; private set; }
        public string UpdatedBy { get; private set; }

        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

        private Department() { }

        public Department(string id, string name, string createdBy)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));

            Status = DepartmentStatus.Active;
            ProvisioningAt = DateTimeOffset.UtcNow;
            UpdatedAt = ProvisioningAt;
            UpdatedBy = createdBy;

            AddEvent(new DepartmentCreatedEvent(
                occurredOn: ProvisioningAt,
                departmentId: id,
                aggregateType: nameof(Department),
                createdBy));
        }

        public void UpdateName(string name, string updatedBy)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name;
                PrepareForUpdate(updatedBy);
            }
        }

        public void ChangeStatus(DepartmentStatus newStatus, string updatedBy)
        {
            if (Status == newStatus) return;

            Status = newStatus;
            PrepareForUpdate(updatedBy);
        }

        public void PrepareForUpdate(string updatedBy)
        {
            UpdatedAt = DateTimeOffset.UtcNow;
            UpdatedBy = updatedBy;
        }

        public void AddEvent(Event domainEvent)
        {
            if (domainEvent != null)
                _events.Add(domainEvent);
        }

        public void ClearEvents() => _events.Clear();

        public void AddNotification(Notification notification)
        {
            if (notification != null)
                _notifications.Add(notification);
        }

        public void AddNotifications(IEnumerable<Notification> notifications)
        {
            if (notifications != null)
                _notifications.AddRange(notifications);
        }
    }

}
