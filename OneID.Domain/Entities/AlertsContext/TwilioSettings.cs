#nullable disable
namespace OneID.Domain.Entities.AlertsContext
{
    public sealed class TwilioSettings
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string FromPhoneNumber { get; set; }
    }

}
