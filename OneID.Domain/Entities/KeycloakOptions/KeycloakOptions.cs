#nullable disable
using OneID;

namespace OneID.Domain.Entities.KeycloakOptions
{
    public sealed class KeycloakOptions
    {
        public string BaseUrl { get; set; }
        public string Realm { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

}
