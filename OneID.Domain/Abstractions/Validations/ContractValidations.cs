using OneID.Domain.Contracts.Validations;
using OneID.Domain.Notifications;

#nullable disable

namespace OneID.Domain.Abstractions.Validations
{
    public partial class ContractValidations<T> where T : IContract
    {
        private readonly object _lock = new();

        private List<Notification> _notifications = [];
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();
        public void AddNotification(Notification notification)
        {
            lock (_lock)
            {
                _notifications.Add(notification);
            }
        }
        public ContractValidations() { }
        public ContractValidations(IEnumerable<Notification> notifications)
        {
            if (_notifications == null) return;

            lock (_lock)
            {
                _notifications.AddRange(notifications);
            }
        }
        public bool HasNotifications() => !_notifications.Any();
    }
}
