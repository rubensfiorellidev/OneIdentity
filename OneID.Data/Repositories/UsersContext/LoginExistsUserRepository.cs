using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;

namespace OneID.Data.Repositories.UsersContext
{
    public class LoginExistsUserRepository : ILoginExistsUserRepository
    {
        private readonly IOneDbContextFactory _contextFactory;

        public LoginExistsUserRepository(IOneDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> LoginExistsAsync(string login, CancellationToken cancellationToken)
        {
            await using var dbContext = _contextFactory.CreateDbContext();

            return await dbContext.UserAccounts
                .AnyAsync(u => u.Login == login, cancellationToken);
        }
    }

}
