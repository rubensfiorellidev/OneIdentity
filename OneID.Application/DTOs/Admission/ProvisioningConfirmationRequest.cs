#nullable disable
namespace OneID.Application.DTOs.Admission
{
    public record ProvisioningConfirmationRequest
    {
        public Guid CorrelationId { get; init; }
        public string TotpCode { get; init; }
    }

}
