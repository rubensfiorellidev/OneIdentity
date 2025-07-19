using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Queries.AdmissionQueries;

#nullable disable
namespace OneID.Application.QueryHandlers.AdmissionQueryHandlers
{
    public sealed class GetRecentAdmissionsQueryHandler
        : IQueryHandler<GetRecentAdmissionsQuery, List<RecentAdmissionDto>>
    {
        private readonly IQueryUserAccountRepository _repository;

        public GetRecentAdmissionsQueryHandler(IQueryUserAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RecentAdmissionDto>> Handle(GetRecentAdmissionsQuery query, CancellationToken cancellationToken)
        {
            var accounts = await _repository.GetRecentAdmissionsAsync(query.Limit, cancellationToken);

            return [.. accounts.Select(x => new RecentAdmissionDto
            {
                AccountId = x.Id,
                FullName = x.FullName,
                JobTitleName = x.JobTitleName,
                DepartmentName = x.DepartmentName,
                Company = x.Company,
                LoginManager = x.LoginManager,
                ProvisioningAt = x.ProvisioningAt
            })];
        }
    }

}
