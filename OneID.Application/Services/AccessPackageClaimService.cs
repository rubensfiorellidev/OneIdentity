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
            //busca os packages conforme cargo X departamento
            var items = await _queryRepository.GetAccessPackageItemsByUserContextAsync(
                user.DepartmentId,
                user.JobTitleId,
                cancellationToken);

            //monta a claim
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
