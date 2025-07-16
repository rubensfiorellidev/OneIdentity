namespace OneID.Application.DTOs.Auth
{
#nullable disable
    public record LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
