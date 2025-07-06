namespace OneID.Domain.Results
{
    public sealed class AuthResult
    {
        public string? Jwtoken { get; set; }
        public string? RefreshToken { get; set; }
        public bool Result { get; set; }

    }
}
