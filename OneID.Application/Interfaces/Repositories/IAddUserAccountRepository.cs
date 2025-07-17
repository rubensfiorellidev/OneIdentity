using OneID.Domain.Entities.UserContext;
using OneID.Domain.Results;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IAddUserAccountRepository
    {
        Task<IResult> AddAsync(UserAccount entity, CancellationToken cancellationToken);

    }
}
