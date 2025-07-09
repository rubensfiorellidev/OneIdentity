#nullable disable
namespace OneID.Application.DTOs.Alerts
{
    public record UpdateAlertSettingsRequest
    {
        public List<string> CriticalRecipients { get; set; } = [];
        public List<string> WarningRecipients { get; set; } = [];
        public List<string> InfoRecipients { get; set; } = [];
    }

}
