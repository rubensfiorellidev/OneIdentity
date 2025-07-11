using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.Services
{
    public interface IUserClaimWriterRepository
    {
        Task AddRangeAsync(IEnumerable<UserClaim> claims, CancellationToken cancellationToken);
    }

}
