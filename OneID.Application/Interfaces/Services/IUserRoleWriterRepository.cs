using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.Services
{
    public interface IUserRoleWriterRepository
    {
        Task AddRangeAsync(IEnumerable<UserRole> roles, CancellationToken cancellationToken);
    }

}
