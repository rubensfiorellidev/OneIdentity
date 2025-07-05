using OneID.Domain.Abstractions.Events;
using OneID.Domain.Contracts;
using OneID.Domain.Notifications;

#nullable disable

namespace OneID.Domain.Entities.UserContext
{
    public abstract class BaseEntity : IValidation
    {
        protected List<Notification> _notifications = [];
        private readonly object _lock = new();

        private readonly List<Event> _events = [];
        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();

        protected void AddEvent(Event domainEvent)
        {
            _events.Add(domainEvent);
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        protected BaseEntity(List<Notification> notifications) => _notifications = notifications ?? [];
        
        public string Id { get; protected init; }

        private DateTimeOffset _provisioningAt;
        public DateTimeOffset ProvisioningAt
        {
            get { return _provisioningAt; }
            set { _provisioningAt = value; }
        }

        private DateTimeOffset? _updatedAt;
        public DateTimeOffset? UpdatedAt
        {
            get { return _updatedAt; }
            set { _updatedAt = value; }
        }

        private string _createdBy;
        public string CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        private string _updatedBy;
        public string UpdatedBy
        {
            get { return _updatedBy; }
            set { _updatedBy = value; }
        }

        public override bool Equals(object obj)
        {
            return obj is BaseEntity entity && entity.Id == Id;
        }
        public override int GetHashCode() => Id.GetHashCode();
        public IReadOnlyCollection<Notification> Notifications => _notifications;
        protected void AddNotificationToStack(List<Notification> notifications)
        {
            if (!notifications.Any())
            {
                return;
            }

            lock (_lock)
            {
                _notifications.AddRange(notifications);
            }
        }

        public abstract bool Validation();
        public void PrepareForUpdate(string updatedBy)
        {
            UpdatedAt = DateTimeOffset.UtcNow;
            UpdatedBy = string.IsNullOrEmpty(updatedBy) ? null : updatedBy;
        }


    }
}
