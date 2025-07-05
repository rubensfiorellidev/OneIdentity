#nullable disable
namespace OneID.Domain.Entities.UserContext
{
    using Microsoft.AspNetCore.Identity;

    public sealed class ApplicationUser : IdentityUser
    {
        public string LoginHash { get; private set; }
        public string LoginCrypt { get; private set; }
        public DateTimeOffset ProvisioningAt { get; private set; }
        public bool IsActive { get; private set; }
        public DateTimeOffset? LastLoginAt { get; private set; }
        public string CreatedBy { get; private set; }
        public string KeycloakUserId { get; private set; }

        private ApplicationUser() { }

        public ApplicationUser(
            string userName,
            string email,
            string phoneNumber,
            string createdBy,
            string loginHash,
            string loginCrypt,
            string keycloakUserId)
        {
            Id = $"{Ulid.NewUlid()}";
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            CreatedBy = createdBy;
            LoginHash = loginHash;
            LoginCrypt = loginCrypt;
            ProvisioningAt = DateTimeOffset.UtcNow;
            IsActive = true;
            KeycloakUserId = keycloakUserId;
        }

        public void SetLastLoginAt(DateTimeOffset lastLogin) => LastLoginAt = lastLogin;
        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
        public void SetKeycloakUserId(string keycloakUserId)
        {
            if (string.IsNullOrWhiteSpace(KeycloakUserId))
                KeycloakUserId = keycloakUserId;
        }
    }

}
