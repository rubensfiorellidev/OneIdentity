using OneID.Application.Interfaces.Graph;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
#nullable disable
namespace OneID.Application.Services.Graph
{
    public sealed class AccessPackageGroupService : IAccessPackageGroupService
    {
        private readonly IAccessPackageClaimService _claimService;
        private readonly IQueryUserAccountRepository _userAccountRepository;

        public AccessPackageGroupService(IAccessPackageClaimService claimService, IQueryUserAccountRepository userAccountRepository)
        {
            _claimService = claimService;
            _userAccountRepository = userAccountRepository;
        }

        public async Task<IReadOnlyCollection<string>> ResolveGroupsForUserAsync(Guid correlationId, CancellationToken cancellationToken)
        {
            var userAccount = await _userAccountRepository.GetByCorrelationIdAsync(correlationId, cancellationToken);

            var claims = await _claimService.ResolveClaimsForUserAsync(userAccount, cancellationToken);

            var groupNames = claims
                .Where(c => c.Type == "package:ad")
                .Select(c => c.Value)
                .Distinct()
                .ToList();

            return groupNames;
        }
    }

}
