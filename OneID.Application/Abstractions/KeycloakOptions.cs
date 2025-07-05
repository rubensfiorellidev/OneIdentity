#nullable disable
namespace OneID.Application.Abstractions
{
    public sealed class KeycloakOptions
    {
        public string BaseUrl { get; set; }
        public string Realm { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

}
