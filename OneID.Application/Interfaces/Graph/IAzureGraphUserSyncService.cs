using OneID.Application.Messaging.Sagas.Contracts.Events;
using OneID.Domain.Results;

namespace OneID.Application.Interfaces.Graph
{
    public interface IAzureGraphUserSyncService
    {
        Task<Result> CreateUserAsync(AzureUserCreationRequested request, CancellationToken cancellationToken);
    }


}
