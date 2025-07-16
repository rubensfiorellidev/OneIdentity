using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Abstractions.Validations;
using OneID.Domain.Contracts;
using OneID.Domain.Contracts.Validations;
using OneID.Domain.Notifications;

namespace OneID.Domain.Entities.UserContext
{
#nullable disable
    public class Role : IAuditableEntity, IValidation, IContract
    {
        private readonly List<Event> _events = [];
        private readonly List<Notification> _notifications = [];

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }

        public DateTimeOffset ProvisioningAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();


        private Role() { }

        public Role(string name, string description)
        {
            Id = Ulid.NewUlid().ToString();
            Name = name;
            Description = description;
            IsActive = true;
            ProvisioningAt = DateTimeOffset.UtcNow;
        }

        public void Deactivate() => IsActive = false;

        public void Activate() => IsActive = true;

        public void SetCreatedBy(string createdBy)
        {
            CreatedBy = createdBy;
        }

        public void PrepareForUpdate(string updatedBy)
        {
            UpdatedBy = updatedBy;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void AddNotifications(IEnumerable<Notification> notifications)
        {
            if (notifications != null)
                _notifications.AddRange(notifications);
        }

        public bool IsValid()
        {
            var contracts = RoleValidator.Validate(this);
            AddNotifications(contracts.Notifications);
            return contracts.HasNotifications();
        }
    }

}
