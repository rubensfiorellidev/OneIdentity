namespace OneID.Application.DTOs.ActiveSessions
{
    public record ActiveSessionInfo
    {
        public string CircuitId { get; }
        public string IpAddress { get; }
        public string UpnOrName { get; }
        public string UserAgent { get; }
        public DateTimeOffset LastActivity { get; }
        public DateTimeOffset ExpiresAt { get; }
        public TimeSpan TimeToExpire => ExpiresAt - DateTimeOffset.UtcNow;

        public ActiveSessionInfo(
            string circuitId,
            string ipAddress,
            string upnOrName,
            string userAgent,
            DateTimeOffset lastActivity,
            DateTimeOffset expiresAt)
        {
            CircuitId = circuitId;
            IpAddress = ipAddress;
            UpnOrName = upnOrName;
            UserAgent = userAgent;
            LastActivity = lastActivity;
            ExpiresAt = expiresAt;
        }


    }


}
