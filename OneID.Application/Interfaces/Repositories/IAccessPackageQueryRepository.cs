using OneID.Application.DTOs.Admission;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IAccessPackageQueryRepository
    {
        Task<IEnumerable<AccessPackageItemDto>> GetAccessPackageItemsByUserContextAsync(string department, string jobTitle, CancellationToken cancellationToken);
    }

}
