#nullable disable
namespace OneID.Domain.Entities.UserContext
{
    public class AdmissionAlert
    {
        public string Id { get; set; } = $"{Ulid.NewUlid()}";
        public string CpfHash { get; set; }
        public string FullName { get; set; }
        public string PositionHeldId { get; set; }
        public string WarningMessage { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
