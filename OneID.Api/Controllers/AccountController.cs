using MediatR;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces;

namespace OneID.Api.Controllers
{
    [Route("v1/json/accounts")]
    public class AccountController : MainController
    {
        private readonly IAccountProvisioningOrchestrator _orchestrator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ISender sender,
            IAccountProvisioningOrchestrator orchestrator,
            ILogger<AccountController> logger) : base(sender)
        {
            _orchestrator = orchestrator;
            _logger = logger;
        }

        [HttpPost("start-provisioning")]
        public async Task<IActionResult> StartProvisioningAsync([FromBody] AccountRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return UnprocessableEntity(ModelState);

                var login = await _orchestrator.ProvisionLoginAsync(request.Firstname, request.Lastname, cancellationToken);

                return Ok(new
                {
                    Message = "Provisionamento concluído com sucesso",
                    Login = login
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao provisionar usuário");
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }

}
