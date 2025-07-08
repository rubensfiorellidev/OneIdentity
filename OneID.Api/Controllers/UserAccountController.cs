using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.Commands;
using OneID.Application.DTOs.Admission;

namespace OneID.Api.Controllers
{
    [Route("v1/accounts")]
    public class UserAccountController : MainController
    {
        private readonly ILogger<UserAccountController> _logger;

        public UserAccountController(
            ISender sender,
            ILogger<UserAccountController> logger) : base(sender)
        {
            _logger = logger;
        }

        [HttpPost("start-provisioning")]
        [Authorize]
        public async Task<IActionResult> StartProvisioningAsync([FromBody] AccountRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return UnprocessableEntity(ModelState);

                var command = new CreateAccountStagingCommand(request);
                var result = await Sender.Send(command, cancellationToken);

                return result.Match(
                    success => Accepted(new { success.Message, success.Data }),
                    error => Problem(detail: error.Message, statusCode: error.HttpCode ?? 500)
                );

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar o processo de provisionamento");
                return Problem(detail: "Erro inesperado ao iniciar o processo", statusCode: 500);
            }
        }
    }

}
