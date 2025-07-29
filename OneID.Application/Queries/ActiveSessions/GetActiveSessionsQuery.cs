using OneID.Application.DTOs.ActiveSessions;
using OneID.Application.Interfaces.CQRS;

namespace OneID.Application.Queries.ActiveSessions
{
    public record GetActiveSessionsQuery : IQuery<List<SessionTelemetry>>;

}
