using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.Services
{
    public interface IAccessPackageClaimService
    {
        Task<IEnumerable<UserClaim>> ResolveClaimsForUserAsync(UserAccount user, CancellationToken cancellationToken);
    }

}
