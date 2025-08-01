using MediatR;
using Microsoft.Extensions.Logging;
using OneID.Application.DTOs.Users;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Queries.Users;
using OneID.Domain.Abstractions.DomainErrorCodes;
using System.Runtime.CompilerServices;

#nullable disable
namespace OneID.Application.QueryHandlers.Users
{
    public sealed class GetAllUsersQueryHandler : IStreamRequestHandler<GetAllUsersQuery, UserResponse>
    {
        private readonly IQueryUserRepository _repository;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(IQueryUserRepository repository, ILogger<GetAllUsersQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async IAsyncEnumerable<UserResponse> Handle(
            GetAllUsersQuery request,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            LogRequest("Iniciando consulta paginada de usuários", request);

            IAsyncEnumerable<UserResponse> users;
            try
            {
                users = _repository.GetUsersPagedAsync(request.Page, request.PageSize, cancellationToken);
            }
            catch (Exception ex)
            {
                var errorCode = ErrorCodes.DbConnectionFailed;
                var errorMessage = ErrorCodes.GetErrorMessageByCategory(errorCode, ErrorCodes.ErrorCategory.Database);

                ex.Data["ErrorCode"] = errorCode;
                ex.Data["ErrorMessage"] = errorMessage;

                _logger.LogError(ex, "{ErrorCode}: {ErrorMessage} ao consultar usuários", errorCode, errorMessage);
                throw;
            }

            int count = 0;

            await foreach (var user in users.WithCancellation(cancellationToken))
            {
                count++;
                yield return user;
            }

            _logger.LogInformation("Consulta concluída com {Count} usuários. Page: {Page}, PageSize: {PageSize}",
                count, request.Page, request.PageSize);
        }

        private void LogRequest(string message, GetAllUsersQuery request)
        {
            _logger.LogInformation(
                "{Message} | Query: {Query} | Page: {Page} | PageSize: {PageSize} | Timestamp: {UtcNow}",
                message,
                nameof(GetAllUsersQuery),
                request.Page,
                request.PageSize,
                DateTimeOffset.UtcNow);
        }
    }

}
