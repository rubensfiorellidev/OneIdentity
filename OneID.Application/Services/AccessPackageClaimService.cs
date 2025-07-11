using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Services
{
    public sealed class AccessPackageClaimService : IAccessPackageClaimService
    {
        private readonly IAccessPackageQueryRepository _queryRepository;

        public AccessPackageClaimService(IAccessPackageQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<IEnumerable<UserClaim>> ResolveClaimsForUserAsync(UserAccount user, CancellationToken cancellationToken)
        {
            var items = await _queryRepository.GetAccessPackageItemsByUserContextAsync(
                user.Department,
                user.JobTitleId,
                cancellationToken);

            var claims = items.Select(item =>
                new UserClaim(
                    user.Id,
                    $"package:{item.Type.ToLowerInvariant()}",
                    item.Value
                ));

            return claims;
        }
    }

}
