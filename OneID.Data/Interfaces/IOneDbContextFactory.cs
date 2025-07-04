using OneID.Data.DataContexts;

namespace OneID.Application.Interfaces
{
    public interface IOneDbContextFactory
    {
        OneDbContext CreateDbContext();

    }
}
