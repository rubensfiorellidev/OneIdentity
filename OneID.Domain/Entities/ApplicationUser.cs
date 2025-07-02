#nullable disable
namespace OneID.Domain.Entities
{
    using Microsoft.AspNetCore.Identity;

    public sealed class ApplicationUser : IdentityUser
    {
        public string Fullname { get; private set; }
        public string LoginHash { get; private set; }
        public string LoginCrypt { get; private set; }
        public DateTimeOffset ProvisioningAt { get; private set; }
        public bool IsActive { get; private set; }
        public DateTimeOffset? LastLoginAt { get; private set; }
        public string CreatedBy { get; private set; }

        private ApplicationUser() { }
        public ApplicationUser(
            string fullName,
            string email,
            string phoneNumber,
            string createdBy,
            string loginHash,
            string loginCrypt)
        {
            Id = $"{Ulid.NewUlid()}";
            Fullname = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            CreatedBy = createdBy;
            LoginHash = loginHash;
            LoginCrypt = loginCrypt;
            ProvisioningAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }

        public void SetLastLoginAt(DateTimeOffset lastLogin) => LastLoginAt = lastLogin;
        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;

    }

}
