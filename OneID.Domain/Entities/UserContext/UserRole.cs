namespace OneID.Domain.Entities.UserContext
{
#nullable disable
    public class UserRole
    {
        public string UserAccountId { get; private set; }
        public string RoleId { get; private set; }

        public UserAccount User { get; private set; }
        public Role Role { get; private set; }

        private UserRole() { }

        public UserRole(string userId, string roleId)
        {
            UserAccountId = userId;
            RoleId = roleId;
        }
    }

}
