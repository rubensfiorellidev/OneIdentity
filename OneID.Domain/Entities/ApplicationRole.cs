using Microsoft.AspNetCore.Identity;

#nullable disable
namespace OneID.Domain.Entities
{
    public sealed class ApplicationRole : IdentityRole
    {
        public string Description { get; private set; }

    }
}
