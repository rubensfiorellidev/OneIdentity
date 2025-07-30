namespace OneID.Application.DTOs.ActiveSessions
{
    public record ActiveSessionInfo(
    string CircuitId,
    string IpAddress,
    string UpnOrName,
    DateTime LastActivity,
    DateTime ExpiresAt
    );

}
