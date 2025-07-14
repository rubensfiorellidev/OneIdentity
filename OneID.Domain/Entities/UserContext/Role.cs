namespace OneID.Domain.Entities.UserContext
{
#nullable disable
    public class Role
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }

        private Role() { }

        public Role(string name, string description)
        {
            Id = Ulid.NewUlid().ToString();
            Name = name;
            Description = description;
            IsActive = true;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }

}
