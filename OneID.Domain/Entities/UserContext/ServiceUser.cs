namespace OneID.Domain.Entities.UserContext
{
    public sealed class ServiceUser
    {
        public string Id { get; private set; } = Ulid.NewUlid().ToString();
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

        private readonly List<ServiceUserClaim> _claims = [];
        public IReadOnlyCollection<ServiceUserClaim> Claims => _claims.AsReadOnly();

        public ServiceUser(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public void AddClaim(string type, string value)
        {
            _claims.Add(new ServiceUserClaim(Id, type, value));
        }

        public void Deactivate() => IsActive = false;
    }

}
