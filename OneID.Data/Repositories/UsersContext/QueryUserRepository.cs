using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneID.Application.DTOs.Users;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using System.Runtime.CompilerServices;

namespace OneID.Data.Repositories.UsersContext
{
    public sealed class QueryUserRepository : IQueryUserRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;
        private readonly ILogger<QueryUserRepository> _logger;
        private readonly IRedisRepository _redis;
        public QueryUserRepository(IOneDbContextFactory dbContextFactory, ILogger<QueryUserRepository> logger, IRedisRepository redis)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _redis = redis;
        }

        public async IAsyncEnumerable<UserResponse> GetUsersPagedAsync(int page, int pageSize, [EnumeratorCancellation] CancellationToken ct)
        {
            var cacheKey = $"users:page:{page}:size:{pageSize}";

            // 1. Tenta obter os usuários do cache
            if (page == 0)
            {
                var cached = await _redis.GetAsync<List<UserResponse>>(cacheKey);
                if (cached is not null)
                {
                    _logger.LogInformation("Cache hit para {CacheKey}", cacheKey);
                    foreach (var user in cached)
                        yield return user;

                    yield break;
                }
                _logger.LogInformation("Cache miss para {CacheKey}", cacheKey);
            }

            // 2. Se não tiver no cache, busca no banco de dados
            await using var dbContext = _dbContextFactory.CreateDbContext();

            var query = dbContext.UserAccounts
                .AsNoTracking()
                .OrderBy(u => u.LastName)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Company = u.Company,
                    StatusUserAccount = u.StatusUserAccount,
                    TypeUserAccount = u.TypeUserAccount,
                    JobTitleName = u.JobTitleName,
                    CreatedBy = u.CreatedBy
                })
                .AsAsyncEnumerable();

            var buffer = new List<UserResponse>(pageSize);
            int count = 0;

            await foreach (var user in query.WithCancellation(ct))
            {
                buffer.Add(user);
                yield return user;
                count++;
            }

            _logger.LogInformation("Retrieved {Count} users from the database.", count);

            // 3. Só cacheia a primeira página
            if (page == 0 && buffer.Count > 0)
            {
                await _redis.SetAsync(cacheKey, buffer, TimeSpan.FromSeconds(40));
                _logger.LogInformation("Dados da primeira página cacheados em {CacheKey}", cacheKey);
            }
        }
    }
}
