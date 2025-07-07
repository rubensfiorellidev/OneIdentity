using OneID.Application.Interfaces.Services;
using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IAddUserAccountRepository
    {
        Task<IResult> AddAsync(UserAccount entity, CancellationToken cancellationToken);

    }
}
