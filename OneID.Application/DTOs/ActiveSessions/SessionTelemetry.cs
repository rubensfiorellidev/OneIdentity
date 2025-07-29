namespace OneID.Application.DTOs.ActiveSessions
{
    public record SessionTelemetry(
    string CircuitId,
    string IpAddress,
    string UpnOrName,
    DateTime LastActivity,
    DateTime ExpiresAt
    );

}
