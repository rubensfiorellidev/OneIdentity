using Microsoft.AspNetCore.Identity;

#nullable disable
namespace OneID.Domain.Entities.UserContext
{
    public sealed class ApplicationRole : IdentityRole
    {
        public string Description { get; private set; }

    }
}
