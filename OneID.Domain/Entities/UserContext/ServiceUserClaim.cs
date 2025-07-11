namespace OneID.Domain.Entities.UserContext
{
    public sealed class ServiceUserClaim
    {
        public string Id { get; private set; } = Ulid.NewUlid().ToString();
        public string ServiceUserId { get; private set; }
        public string Type { get; private set; }
        public string Value { get; private set; }

        public ServiceUserClaim(string serviceUserId, string type, string value)
        {
            ServiceUserId = serviceUserId;
            Type = type;
            Value = value;
        }
    }

}
