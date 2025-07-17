using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Interfaces;

namespace OneID.Domain.Notifications
{

#nullable disable
    public class Notification : Event, INotification
    {
        private readonly List<string> _messages = [];

        public Notification(
            string message,
            string propertyName = null,
            object data = null,
            bool? success = null,
            int statusCode = 200,
            int currentVersion = 1) : base()
        {
            Version = currentVersion;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            PropertyName = propertyName;
            Data = data;
            Success = success ?? false;
            StatusCode = statusCode;
        }
        public string Message { get; private set; }
        public string PropertyName { get; private set; }
        public object Data { get; private set; }
        public bool Success { get; private set; }
        public int StatusCode { get; private set; }
        public IReadOnlyCollection<string> Messages => _messages.AsReadOnly();
        public void AddMessage(string message) => _messages.Add(message);
    }
}