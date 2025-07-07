namespace OneID.Application.Interfaces.Repositories
{
    public interface ILoginExistsUserRepository
    {
        Task<bool> LoginExistsAsync(string login, CancellationToken ct);

    }
}
