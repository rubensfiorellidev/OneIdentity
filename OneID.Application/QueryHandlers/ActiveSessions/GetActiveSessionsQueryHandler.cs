using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneID.Application.DTOs.ActiveSessions;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Queries.ActiveSessions;
using StackExchange.Redis;

namespace OneID.Application.QueryHandlers.ActiveSessions
{
    public class GetActiveSessionsQueryHandler : IQueryHandler<GetActiveSessionsQuery, List<ActiveSessionInfo>>
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<GetActiveSessionsQueryHandler> _logger;

        public GetActiveSessionsQueryHandler(IConnectionMultiplexer redis, ILogger<GetActiveSessionsQueryHandler> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        public async Task<List<ActiveSessionInfo>> Handle(GetActiveSessionsQuery query, CancellationToken cancellationToken)
        {
            var db = _redis.GetDatabase();

            var server = _redis
                .GetEndPoints()
                .Select(ep => _redis.GetServer(ep))
                .FirstOrDefault(s => !s.IsReplica && s.IsConnected);

            if (server is null)
                return [];

            var keys = server.Keys(pattern: "session:*").ToArray();

            var now = DateTimeOffset.UtcNow;
            var sessions = new List<ActiveSessionInfo>();

            foreach (var key in keys)
            {
                var json = await db.StringGetAsync(key);
                if (!json.HasValue)
                    continue;

                try
                {
                    var session = JsonConvert.DeserializeObject<ActiveSessionInfo>(json!);
                    if (session is not null && session.ExpiresAt > now)
                        sessions.Add(session);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Erro ao desserializar sessão: {ex.Message}");
                }
            }

            return [.. sessions.OrderByDescending(s => s.LastActivity)];
        }
    }

}
