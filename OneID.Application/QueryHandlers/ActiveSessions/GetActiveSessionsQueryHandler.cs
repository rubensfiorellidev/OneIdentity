using Newtonsoft.Json;
using OneID.Application.DTOs.ActiveSessions;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Queries.ActiveSessions;
using StackExchange.Redis;

namespace OneID.Application.QueryHandlers.ActiveSessions
{
    public class GetActiveSessionsQueryHandler : IQueryHandler<GetActiveSessionsQuery, List<SessionTelemetry>>
    {
        private readonly IConnectionMultiplexer _redis;

        public GetActiveSessionsQueryHandler(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<List<SessionTelemetry>> Handle(GetActiveSessionsQuery query, CancellationToken cancellationToken)
        {
            var db = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: "session:*").ToArray();

            var sessions = new List<SessionTelemetry>();

            foreach (var key in keys)
            {
                var json = await db.StringGetAsync(key);
                if (!json.HasValue)
                    continue;

                var session = JsonConvert.DeserializeObject<SessionTelemetry>(json!);
                if (session is not null)
                    sessions.Add(session);
            }

            var ordered = sessions
                .Where(s => s.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(s => s.LastActivity)
                .ToList();

            return ordered;

        }
    }

}
