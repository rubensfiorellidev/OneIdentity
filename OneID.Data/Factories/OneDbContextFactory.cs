using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces;
using OneID.Data.DataContexts;

#nullable disable
namespace OneID.Data.Factories
{
    internal class OneDbContextFactory : IOneDbContextFactory
    {
        private readonly DbContextOptions<OneDbContext> _options;

        public OneDbContextFactory(DbContextOptions<OneDbContext> options) => _options = options;

        public OneDbContext CreateDbContext() => new(_options);

    }
}
