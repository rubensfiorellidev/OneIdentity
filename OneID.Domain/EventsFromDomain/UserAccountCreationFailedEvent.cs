using OneID.Domain.Abstractions.EventsContext;

namespace OneID.Domain.EventsFromDomain
{
    public sealed class UserAccountCreationFailedEvent : Event
    {
        private const string EntityTypeKey = "EntityType";
        private const string ActionKey = "Action";
        private const string AccountIdKey = "AccountId";
        private const string CreatedByKey = "CreatedBy";
        private const string AggregateTypeKey = "AggregateType";
        private const string DescriptionKey = "Description";

        public UserAccountCreationFailedEvent(DateTimeOffset occurredOn,
                                      string accountId,
                                      string aggregateType,
                                      string createdBy)
                                      : base($"{Ulid.NewUlid()}", occurredOn, 1)
        {
            AccountId = accountId;
            AggregateType = aggregateType;
            Description = $"Failed to create {accountId} created by {createdBy}.";

            Metadata[EntityTypeKey] = "UserAccount";
            Metadata[ActionKey] = "Failed";
            Metadata[AccountIdKey] = AccountId;
            Metadata[CreatedByKey] = CreatedBy;
            Metadata[AggregateTypeKey] = AggregateType;
            Metadata[DescriptionKey] = Description;
        }
        public string AccountId { get; init; }
        public string AggregateType { get; init; }
    }
}
