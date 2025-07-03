using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces;
using OneID.Data.Factories;

namespace OneID.Data.Repositories.UsersContext
{
    public class UserRepository : IUserRepository
    {
        private readonly IOneDbContextFactory _contextFactory;

        public UserRepository(IOneDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> LoginExistsAsync(string login, CancellationToken cancellationToken)
        {
            await using var dbContext = _contextFactory.CreateDbContext();

            return await dbContext.Users
                .AnyAsync(u => u.LoginCrypt == login, cancellationToken);
        }
    }

}
