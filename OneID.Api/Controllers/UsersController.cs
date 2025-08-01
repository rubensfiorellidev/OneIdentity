using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.DTOs.Users;
using OneID.Application.Queries.Users;
using Serilog;

namespace OneID.Api.Controllers
{
    [Route("v1/users")]
    public class UsersController : MainController
    {
        private readonly ILogger<UsersController> _logger;
        public UsersController(ISender sender, ILogger<UsersController> logger) : base(sender)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        [Route("", Name = nameof(GetUsersAsync))]
        public async Task<IActionResult> GetUsersAsync([FromQuery] int page = 0,
                                                             [FromQuery] int pageSize = 25,
                                                             CancellationToken ct = default)
        {

            try
            {
                ct.ThrowIfCancellationRequested();

                var query = new GetAllUsersQuery(page, pageSize);
                var stream = Sender.CreateStream(query, ct);

                var result = new List<UserResponse>(pageSize);
                int count = 0;

                await foreach (var response in stream.WithCancellation(ct))
                {
                    if (response is null)
                    {
                        Log.Warning("Error processing account {ErrorMessage}", response);

                        return Problem(
                            detail: "Erro interno ao processar item do stream.",
                            statusCode: 500,
                            title: "Erro na consulta de usuários"
                        );

                    }
                    result.Add(response);
                    count++;
                }

                _logger.LogInformation("Consulta de usuários concluída com {Count} resultados (Page: {Page}, PageSize: {PageSize})",
                    count, page, pageSize);

                return ResponseOk($"Total de usuários retornados: {count}", result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Requisição cancelada pelo cliente.");
                return StatusCode(499, "Requisição cancelada pelo cliente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuários (Page: {Page}, PageSize: {PageSize})", page, pageSize);
                return InternalServerError("Erro inesperado ao consultar os usuários.");
            }

        }

    }
}
