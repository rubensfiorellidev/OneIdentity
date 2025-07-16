using OneID.Domain.Abstractions.EventsContext;

#nullable disable
namespace OneID.Domain.EventsFromDomain
{
    public sealed class DepartmentCreatedEvent : Event
    {
        private const string EntityTypeKey = "EntityType";
        private const string ActionKey = "Action";
        private const string DepartmentIdKey = "DepartmentId";
        private const string CreatedByKey = "CreatedBy";
        private const string AggregateTypeKey = "AggregateType";
        private const string DescriptionKey = "Description";

        public DepartmentCreatedEvent(DateTimeOffset occurredOn,
                                      string departmentId,
                                      string aggregateType,
                                      string createdBy)
                                      : base($"{Ulid.NewUlid()}", occurredOn, 1)
        {
            DepartmentId = departmentId;
            AggregateType = aggregateType;
            Description = $"Account {departmentId} created by {createdBy}.";

            Metadata[EntityTypeKey] = "UserAccount";
            Metadata[ActionKey] = "Created";
            Metadata[DepartmentIdKey] = DepartmentId;
            Metadata[CreatedByKey] = createdBy;
            Metadata[AggregateTypeKey] = AggregateType;
            Metadata[DescriptionKey] = Description;
        }
        public string DepartmentId { get; init; }
        public string AggregateType { get; init; }
    }
}
