namespace OneID.Application.DTOs.ActiveSessions
{
    public sealed record ActiveSessionDto(
    string CircuitId,
    string IpAddress,
    string UpnOrName,
    DateTime LastActivity,
    DateTime ExpiresAt);

}
