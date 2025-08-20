#nullable disable
namespace OneID.Shared.Authentication
{
    public record KeyMetadata(
     DateTimeOffset CreatedAt,
     int KeySize,
     string KeyId,
     string KeyVersion,
     string Salt
    );

}
