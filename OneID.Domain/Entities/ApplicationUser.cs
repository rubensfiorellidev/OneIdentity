using Microsoft.AspNetCore.Identity;

#nullable disable
namespace OneID.Domain.Entities
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string Fullname { get; private set; }
        public string Login { get; private set; }
    }
}
