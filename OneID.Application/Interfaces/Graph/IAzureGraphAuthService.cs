namespace OneID.Application.Interfaces.Graph
{
    public interface IAzureGraphAuthService
    {
        Task<string> GetAccessTokenAsync();
    }
}
