#nullable disable
namespace OneID.Domain.Entities.UserContext
{
    public class UserClaim
    {
        public string Id { get; private set; }
        public string UserAccountId { get; private set; }

        public string Type { get; private set; }
        public string Value { get; private set; }

        public UserAccount User { get; private set; }

        private UserClaim() { }

        public UserClaim(string userId, string type, string value)
        {
            Id = Ulid.NewUlid().ToString();
            UserAccountId = userId;
            Type = type;
            Value = value;
        }
    }

}
