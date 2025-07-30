namespace OneID.Application.Interfaces.Repositories
{
    public interface IRedisRepository
    {
        ValueTask SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        ValueTask<T?> GetAsync<T>(string key);
        ValueTask<bool> ExistsAsync(string key);
        ValueTask<bool> DeleteAsync(string key);
    }


}
