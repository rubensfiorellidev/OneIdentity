using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;

namespace OneID.Application.Queries.AdmissionQueries
{
    public record GetRecentAdmissionsQuery(int Limit = 50) : IQuery<List<RecentAdmissionDto>>;

}
