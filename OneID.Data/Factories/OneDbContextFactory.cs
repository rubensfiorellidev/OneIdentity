using Microsoft.EntityFrameworkCore;
using OneID.Data.DataContexts;

#nullable disable
namespace OneID.Data.Factories
{
    internal class OneDbContextFactory : IOneDbContextFactory
    {
        private readonly DbContextOptions<OneIdDbContext> _options;

        public OneDbContextFactory(DbContextOptions<OneIdDbContext> options) => _options = options;

        public OneIdDbContext CreateDbContext() => new(_options);

    }
}
