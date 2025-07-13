namespace OneID.Application.Interfaces.Interceptor
{
    public interface ICurrentUserService
    {
        string GetUsername();
        string GetUserId();
        string GetClaim(string claimType);
    }
}
