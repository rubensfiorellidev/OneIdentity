using OneID.Domain.Abstractions.EventsContext;

namespace OneID.Domain.EventsFromDomain
{
    public sealed class UserAccountCreatedEvent : Event
    {
        private const string EntityTypeKey = "EntityType";
        private const string ActionKey = "Action";
        private const string AccountIdKey = "AccountId";
        private const string CreatedByKey = "CreatedBy";
        private const string AggregateTypeKey = "AggregateType";
        private const string DescriptionKey = "Description";

        public UserAccountCreatedEvent(DateTimeOffset occurredOn,
                                      string accountId,
                                      string aggregateType,
                                      string createdBy)
                                      : base($"{Ulid.NewUlid()}", occurredOn, 1)
        {
            AccountId = accountId;
            AggregateType = aggregateType;
            Description = $"Account {accountId} created by {createdBy}.";

            Metadata[EntityTypeKey] = "UserAccount";
            Metadata[ActionKey] = "Created";
            Metadata[AccountIdKey] = AccountId;
            Metadata[CreatedByKey] = createdBy;
            Metadata[AggregateTypeKey] = AggregateType;
            Metadata[DescriptionKey] = Description;
        }
        public string AccountId { get; init; }
        public string AggregateType { get; init; }
    }
}
