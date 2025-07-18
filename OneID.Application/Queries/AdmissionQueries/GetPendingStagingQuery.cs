using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;

namespace OneID.Application.Queries.AdmissionQueries
{
    public record GetPendingStagingQuery : IQuery<List<PendingProcessDto>>
    {
    }
}
