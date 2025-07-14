using OneID.Application.Interfaces.Services;
using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Services
{
    public sealed class AccessPackageRoleService : IAccessPackageRoleService
    {
        public Task<IEnumerable<UserRole>> ResolveRolesForUserAsync(UserAccount user, CancellationToken cancellationToken)
        {
            var roles = new List<UserRole>
            {
                new(user.Id, "user")
            };

            return Task.FromResult(roles.AsEnumerable());
        }
    }

}
