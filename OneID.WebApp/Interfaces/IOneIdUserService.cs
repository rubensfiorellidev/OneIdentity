using OneID.WebApp.ViewModels;

namespace OneID.WebApp.Interfaces
{
    public interface IOneIdUserService
    {
        Task<PaginatedUsersViewModel> GetUsersAsync(int page, int pageSize, string? searchTerm, string? sortBy, bool descending, CancellationToken cancellationToken = default);
    }


}
