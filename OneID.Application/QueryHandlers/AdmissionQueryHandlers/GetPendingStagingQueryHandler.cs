using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Queries.AdmissionQueries;

namespace OneID.Application.QueryHandlers.AdmissionQueryHandlers
{
    public sealed class GetPendingStagingQueryHandler : IQueryHandler<GetPendingStagingQuery, List<PendingProcessDto>>
    {
        private readonly IQueryAccountAdmissionStagingRepository _repository;

        public GetPendingStagingQueryHandler(IQueryAccountAdmissionStagingRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PendingProcessDto>> Handle(GetPendingStagingQuery query, CancellationToken cancellationToken)
        {
            return await _repository.GetPendingAsync(cancellationToken);
        }
    }

}
