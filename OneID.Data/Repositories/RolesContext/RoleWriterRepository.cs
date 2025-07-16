using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Repositories.RolesContext
{
    public class RoleWriterRepository : IRoleWriterRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;

        public RoleWriterRepository(IOneDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            await dbContext.Roles.AddAsync(role, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            dbContext.Roles.Update(role);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(string roleId, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            var role = await dbContext.Roles.FindAsync([roleId], cancellationToken);
            if (role is not null)
            {
                dbContext.Roles.Remove(role);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

}
