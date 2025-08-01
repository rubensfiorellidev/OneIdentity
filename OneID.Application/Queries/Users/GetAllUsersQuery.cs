using MediatR;
using OneID.Application.DTOs.Users;

namespace OneID.Application.Queries.Users
{
    public sealed record GetAllUsersQuery(int Page = 0, int PageSize = 25)
        : IStreamRequest<UserResponse>;

}
