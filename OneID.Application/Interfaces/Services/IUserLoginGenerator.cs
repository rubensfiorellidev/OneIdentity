namespace OneID.Application.Interfaces.Services
{
    public interface IUserLoginGenerator
    {
        Task<string> GenerateLoginAsync(string fullName, CancellationToken ct);
    }

}
