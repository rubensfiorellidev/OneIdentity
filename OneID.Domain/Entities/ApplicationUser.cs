#nullable disable
namespace OneID.Domain.Entities
{
    using Microsoft.AspNetCore.Identity;

    public sealed class ApplicationUser : IdentityUser
    {
        public string Fullname { get; private set; }
        public string Login { get; private set; }
        public DateTimeOffset ProvisioningAt { get; private set; }
        public bool IsActive { get; private set; }
        public DateTimeOffset? LastLoginAt { get; private set; }
        public string CreatedBy { get; private set; }

        public ApplicationUser() { }
        internal ApplicationUser(
            string login,
            string fullName,
            string email,
            string phoneNumber,
            string createdBy)
        {
            Id = $"{Ulid.NewUlid()}";
            UserName = login;
            Login = login;
            Fullname = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            ProvisioningAt = DateTimeOffset.UtcNow;
            IsActive = true;
            CreatedBy = createdBy;

        }

        public void SetLastLoginAt(DateTimeOffset lastLogin)
        {
            LastLoginAt = lastLogin;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }

}
