#nullable disable
namespace OneID.Domain.Entities
{
    using Microsoft.AspNetCore.Identity;

    public sealed class ApplicationUser : IdentityUser
    {
        public string Fullname { get; private set; }
        public string Login { get; private set; }
        public DateTimeOffset ProvisioningAt { get; private set; }

        public ApplicationUser()
        {
            Id = $"{Ulid.NewUlid()}";
            ProvisioningAt = DateTimeOffset.UtcNow;

        }
    }
}
