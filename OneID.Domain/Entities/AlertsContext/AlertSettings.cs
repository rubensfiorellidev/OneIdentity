#nullable disable
namespace OneID.Domain.Entities.AlertsContext
{
    public class AlertSettings
    {
        public string Id { get; set; } = Ulid.NewUlid().ToString();

        public List<string> CriticalRecipients { get; set; } = [];
        public List<string> WarningRecipients { get; set; } = [];
        public List<string> InfoRecipients { get; set; } = [];

    }

}
