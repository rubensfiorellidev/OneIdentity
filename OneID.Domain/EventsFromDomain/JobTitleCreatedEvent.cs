using OneID.Domain.Abstractions.EventsContext;

namespace OneID.Domain.EventsFromDomain
{
    public sealed class JobTitleCreatedEvent : Event
    {
        private const string EntityTypeKey = "EntityType";
        private const string ActionKey = "Action";
        private const string JobTitleIdKey = "JobTitleId";
        private const string CreatedByKey = "CreatedBy";
        private const string AggregateTypeKey = "AggregateType";
        private const string DescriptionKey = "Description";

        public JobTitleCreatedEvent(DateTimeOffset occurredOn,
                                    string jobTitleId,
                                    string aggregateType,
                                    string createdBy)
            : base($"{Ulid.NewUlid()}", occurredOn, 1)
        {
            JobTitleId = jobTitleId;
            AggregateType = aggregateType;
            Description = $"JobTitle {jobTitleId} created by {createdBy}.";

            Metadata[EntityTypeKey] = "JobTitle";
            Metadata[ActionKey] = "Created";
            Metadata[JobTitleIdKey] = JobTitleId;
            Metadata[CreatedByKey] = createdBy;
            Metadata[AggregateTypeKey] = AggregateType;
            Metadata[DescriptionKey] = Description;
        }

        public string JobTitleId { get; init; }
        public string AggregateType { get; init; }
    }

}
