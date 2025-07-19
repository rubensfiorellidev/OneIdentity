namespace OneID.WebApp.Components.Tokens
{
    public interface ITotpTokenGenerator
    {
        string GenerateToken(Dictionary<string, object> claims, TimeSpan expiresIn);
    }

}
