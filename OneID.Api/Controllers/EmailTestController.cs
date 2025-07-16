using Microsoft.AspNetCore.Mvc;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.SES;

#nullable disable
namespace OneID.Api.Controllers
{
    [Route("v1/emails")]
    [ApiController]
    public class EmailTestController : MainController
    {
        private readonly ISesEmailSender _emailSender;
        public EmailTestController(ISender dispatcher, ISesEmailSender emailSender) : base(dispatcher)
        {
            _emailSender = emailSender;
        }

        [HttpPost("test")]
        public async Task<IActionResult> SendTestEmail([FromQuery] string to)
        {

            await _emailSender.SendEmailAsync(
            to,
            "Usuário pronto",
            textBody: "Provisionamento no Keycloak concluído. Acesse: https://oneidsecure.com/continuar"
            );


            return Ok("Email enviado com sucesso.");
        }
    }
}
