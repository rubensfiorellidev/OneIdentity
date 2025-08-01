using OneID.Application.DTOs.Users;

#nullable disable
namespace OneID.Application.Interfaces.Repositories
{
    public interface IQueryUserRepository
    {
        IAsyncEnumerable<UserResponse> GetUsersPagedAsync(int page, int pageSize, CancellationToken ct);
    }

}
