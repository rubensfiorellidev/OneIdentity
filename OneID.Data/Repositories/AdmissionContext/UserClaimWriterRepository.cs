using OneID.Application.Interfaces.Services;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Repositories.AdmissionContext
{
    public sealed class UserClaimWriterRepository : IUserClaimWriterRepository
    {
        private readonly IOneDbContextFactory _contextFactory;

        public UserClaimWriterRepository(IOneDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddRangeAsync(IEnumerable<UserClaim> claims, CancellationToken cancellationToken)
        {
            await using var db = _contextFactory.CreateDbContext();
            db.UserClaims.AddRange(claims);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

}
