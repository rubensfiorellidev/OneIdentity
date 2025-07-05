namespace OneID.Application.Interfaces
{
    public interface IUserLoginGenerator
    {
        Task<string> GenerateLoginAsync(string fullName, CancellationToken ct);
    }

}
