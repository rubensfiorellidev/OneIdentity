using Microsoft.EntityFrameworkCore;
using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;

namespace OneID.Data.Repositories.AdmissionContext
{
    public sealed class AccessPackageQueryRepository : IAccessPackageQueryRepository
    {
        private readonly IOneDbContextFactory _contextFactory;

        public AccessPackageQueryRepository(IOneDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<AccessPackageItemDto>> GetAccessPackageItemsByUserContextAsync(
            string department,
            string jobTitle,
            CancellationToken cancellationToken)
        {
            await using var db = _contextFactory.CreateDbContext();

            var packageIds = await db.AccessPackageConditions
                .Where(x => x.Department == department && x.JobTitle == jobTitle)
                .Select(x => x.AccessPackageId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!packageIds.Any())
                return [];

            return await db.AccessPackageItems
                .Where(x => packageIds.Contains(x.AccessPackageId))
                .Select(x => new AccessPackageItemDto
                {
                    Type = x.Type,
                    Value = x.Value
                })
                .ToListAsync(cancellationToken);
        }
    }

}
