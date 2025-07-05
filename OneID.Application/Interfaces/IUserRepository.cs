namespace OneID.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> LoginExistsAsync(string login, CancellationToken ct);

    }
}
