using OneID.Application.Interfaces.Repositories;
using StackExchange.Redis;
using System.Text.Json;

namespace OneID.Data.Redis
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase _database;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
            };
        }

        public async ValueTask SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await _database.StringSetAsync(key, json, expiry);
        }

        public async ValueTask<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }

        public ValueTask<bool> ExistsAsync(string key)
            => new(_database.KeyExistsAsync(key));

        public ValueTask<bool> DeleteAsync(string key)
            => new(_database.KeyDeleteAsync(key));

    }

}
