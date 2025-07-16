#nullable disable
using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Contracts;
using OneID.Domain.EventsFromDomain;
using OneID.Domain.Notifications;
using OneID.Domain.ValueObjects;

namespace OneID.Domain.Entities.JobTitleContext
{
    public sealed class JobTitle : IAggregateRoot, IAuditableEntity
    {
        private readonly List<Event> _events = [];
        private readonly List<Notification> _notifications = [];

        public string Id { get; private set; }
        public string Name { get; private set; }
        public JobTitleStatus Status { get; private set; }

        public DateTimeOffset ProvisioningAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public string CreatedBy { get; private set; }
        public string UpdatedBy { get; private set; }

        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

        private JobTitle() { }

        public JobTitle(string id, string name, string createdBy)
        {
            Id = id;
            Name = name;
            Status = JobTitleStatus.Active;
            ProvisioningAt = DateTimeOffset.UtcNow;
            UpdatedAt = DateTimeOffset.UtcNow;
            CreatedBy = createdBy;
            UpdatedBy = createdBy;

            AddEvent(new JobTitleCreatedEvent(
                occurredOn: ProvisioningAt,
                jobTitleId: id,
                aggregateType: nameof(JobTitle),
                createdBy));
        }

        public void UpdateName(string name, string updatedBy)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            Name = name;
            PrepareForUpdate(updatedBy);
        }

        public void ChangeStatus(JobTitleStatus newStatus, string updatedBy)
        {
            if (Status == newStatus) return;
            Status = newStatus;
            PrepareForUpdate(updatedBy);
        }

        public void PrepareForUpdate(string updatedBy)
        {
            UpdatedBy = updatedBy;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void AddEvent(Event domainEvent) => _events.Add(domainEvent);
        public void ClearEvents() => _events.Clear();
        public void AddNotification(Notification notification)
        {
            if (notification != null) _notifications.Add(notification);
        }

        public void AddNotifications(IEnumerable<Notification> notifications)
        {
            if (notifications != null) _notifications.AddRange(notifications);
        }

    }

}
