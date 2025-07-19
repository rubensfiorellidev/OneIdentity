namespace OneID.ServerWebApp.Shared.Tokens
{
    public interface ITotpTokenGenerator
    {
        string GenerateToken(Dictionary<string, object> claims, TimeSpan expiresIn);
    }

}
