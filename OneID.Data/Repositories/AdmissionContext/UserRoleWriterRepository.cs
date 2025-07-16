using OneID.Application.Interfaces.Services;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Repositories.AdmissionContext
{
    public sealed class UserRoleWriterRepository : IUserRoleWriterRepository
    {
        private readonly IOneDbContextFactory _contextFactory;

        public UserRoleWriterRepository(IOneDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddRangeAsync(IEnumerable<UserRole> roles, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory.CreateDbContext();

            await context.UserRoles.AddRangeAsync(roles, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
    }

}
