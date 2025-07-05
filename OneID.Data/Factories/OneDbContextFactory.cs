using Microsoft.EntityFrameworkCore;
using OneID.Data.DataContexts;
using OneID.Data.Interfaces;

#nullable disable
namespace OneID.Data.Factories
{
    public class OneDbContextFactory : IOneDbContextFactory
    {
        private readonly IDbContextFactory<OneDbContext> _factory;

        public OneDbContextFactory(IDbContextFactory<OneDbContext> factory)
        {
            _factory = factory;
        }

        public OneDbContext CreateDbContext()
        {
            return _factory.CreateDbContext();
        }
    }
}
