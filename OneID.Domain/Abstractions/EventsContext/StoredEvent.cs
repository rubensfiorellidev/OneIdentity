namespace OneID.Domain.Abstractions.EventsContext
{
#nullable disable
    public sealed class StoredEvent
    {
        public StoredEvent() { }

        public StoredEvent(string id,
                           string aggregateId,
                           string aggregateType,
                           string eventType,
                           string eventData,
                           DateTimeOffset occurredOn,
                           int version,
                           string createdBy,
                           string description = null)
        {
            Id = id;
            AggregateId = aggregateId;
            AggregateType = aggregateType;
            EventType = eventType;
            EventData = eventData;
            OccurredOn = occurredOn;
            Version = version;
            CreatedBy = createdBy;
            Description = description;
        }

        public string Id { get; init; }
        public string AggregateId { get; init; }
        public string AggregateType { get; init; }
        public string EventType { get; init; }
        public string EventData { get; init; }
        public DateTimeOffset OccurredOn { get; init; }
        public int Version { get; init; }
        public string CreatedBy { get; init; }
        public string Description { get; init; }
    }

}
