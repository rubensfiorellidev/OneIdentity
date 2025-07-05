using OneID.Data.DataContexts;

namespace OneID.Data.Interfaces
{
    public interface IOneDbContextFactory
    {
        OneDbContext CreateDbContext();

    }
}
