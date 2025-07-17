#nullable disable
namespace OneID.Domain.Entities.Tokens
{
    public class OneTimeToken
    {
        public string Jti { get; private set; }
        public string CorrelationId { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }
        public bool Used { get; private set; }

        protected OneTimeToken() { }

        public OneTimeToken(string jti, string correlationId, DateTimeOffset expiresAt)
        {
            Jti = jti;
            CorrelationId = correlationId;
            ExpiresAt = expiresAt;
            Used = false;
        }

        public void MarkAsUsed()
        {
            Used = true;
        }

        public bool IsValid()
        {
            return !Used && DateTimeOffset.UtcNow < ExpiresAt;
        }
    }

}
