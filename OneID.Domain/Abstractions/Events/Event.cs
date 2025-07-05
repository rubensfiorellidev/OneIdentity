#nullable disable

using OneID.Domain.Contracts.Notifications;

namespace OneID.Domain.Abstractions.Events
{
    public abstract class Event : INotification
    {
        public string Id { get; protected set; }
        public DateTimeOffset OccurredOn { get; protected set; }
        public int Version { get; protected set; }
        public Dictionary<string, object> Metadata { get; protected set; }
        public string Description { get; protected set; }

        public string CreatedBy => Metadata.TryGetValue("CreatedBy", out var createdBy) ? createdBy?.ToString() : null;
        public string UpdatedBy => Metadata.TryGetValue("UpdatedBy", out var updatedBy) ? updatedBy?.ToString() : null;

        protected Event()
        {
            Id = $"EventID:{Ulid.NewUlid()}";
            OccurredOn = DateTimeOffset.UtcNow;
            Version = 1;
            Metadata = [];
        }

        protected Event(string id, DateTimeOffset occurredOn, int version = 1)
        {
            Id = id;
            OccurredOn = occurredOn;
            Version = version;
            Metadata = [];
        }
        public void IncrementVersion()
        {
            Version += 1;
        }
    }

}
