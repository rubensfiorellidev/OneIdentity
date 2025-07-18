namespace OneID.TotpFrontend.RefreshTokens
{
    public interface ITotpTokenGenerator
    {
        string GenerateToken(Dictionary<string, object> claims, TimeSpan expiresIn);
    }

}
