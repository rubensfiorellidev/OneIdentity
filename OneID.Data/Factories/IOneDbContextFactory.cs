using OneID.Data.DataContexts;

namespace OneID.Data.Factories
{
    public interface IOneDbContextFactory
    {
        OneIdDbContext CreateDbContext();

    }
}
