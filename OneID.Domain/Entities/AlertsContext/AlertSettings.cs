#nullable disable
using Newtonsoft.Json;

namespace OneID.Domain.Entities.AlertsContext
{
    public class AlertSettings
    {
        public string Id { get; set; } = Ulid.NewUlid().ToString();

        public string CriticalRecipientsJson { get; set; } = "[]";
        public string WarningRecipientsJson { get; set; } = "[]";
        public string InfoRecipientsJson { get; set; } = "[]";

        public List<string> CriticalRecipients
        {
            get => JsonConvert.DeserializeObject<List<string>>(CriticalRecipientsJson) ?? [];
            set => CriticalRecipientsJson = JsonConvert.SerializeObject(value);
        }

        public List<string> WarningRecipients
        {
            get => JsonConvert.DeserializeObject<List<string>>(WarningRecipientsJson) ?? [];
            set => WarningRecipientsJson = JsonConvert.SerializeObject(value);
        }

        public List<string> InfoRecipients
        {
            get => JsonConvert.DeserializeObject<List<string>>(InfoRecipientsJson) ?? [];
            set => InfoRecipientsJson = JsonConvert.SerializeObject(value);
        }
    }

}
