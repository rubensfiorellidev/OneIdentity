using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.Services
{
    public interface IAccessPackageRoleService
    {
        Task<IEnumerable<UserRole>> ResolveRolesForUserAsync(UserAccount user, CancellationToken cancellationToken);
    }

}
