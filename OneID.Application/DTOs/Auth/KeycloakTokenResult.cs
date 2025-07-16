namespace OneID.Application.DTOs.Auth
{
#nullable disable
    public record KeycloakTokenResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Sub { get; set; }
    }
}
