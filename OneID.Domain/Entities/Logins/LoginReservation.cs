namespace OneID.Domain.Entities.Logins
{
    public sealed class LoginReservation
    {
        public LoginReservation(string login, Guid correlationId, string status)
        {
            Login = login;
            Status = status;
            CorrelationId = correlationId;
            ReservedAtUtc = DateTimeOffset.UtcNow;
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }

        public string Login { get; private set; }
        public string Status { get; private set; }
        public Guid CorrelationId { get; private set; }
        public DateTimeOffset ReservedAtUtc { get; private set; }
        public DateTimeOffset UpdatedAtUtc { get; private set; }


        public void SetStatus(string status) => Status = status;
    }

}
