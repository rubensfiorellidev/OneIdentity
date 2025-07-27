using OneID.Application.DTOs.Auth;
using OneID.Application.Interfaces.CQRS;

namespace OneID.Application.Queries.Auth
{
    public sealed class GetCurrentUserQuery : IQuery<UserInfoResponse>
    {
    }

}
