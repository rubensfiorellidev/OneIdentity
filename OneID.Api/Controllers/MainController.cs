using Microsoft.AspNetCore.Mvc;
using OneID.Application.Interfaces.CQRS;

#nullable disable
namespace OneID.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        protected readonly ISender Send;
        protected MainController(ISender send) => Send = send;

        protected IActionResult CreateResponse(int httpCode, string message, bool success, object result = null)
        {
            return StatusCode(httpCode, new
            {
                httpCode,
                message,
                success,
                data = result
            });
        }
        protected IActionResult ResponseOk(string message, object result = null)
        {
            return CreateResponse(StatusCodes.Status200OK, message, true, result);
        }

        protected IActionResult ResponseCreated(object result = null)
        {
            return base.Created(string.Empty, result);
        }

        protected IActionResult InternalServerError(string message)
        {
            return CreateResponse(StatusCodes.Status500InternalServerError, message, false);
        }
        protected IActionResult UnprocessableEntity(string message, object result = null)
        {
            return StatusCode(StatusCodes.Status422UnprocessableEntity, result);
        }

        protected IActionResult Fail(int httpCode, string message, object result = null)
        {
            return StatusCode(httpCode, new
            {
                httpCode,
                message,
                data = result
            });
        }

    }
}
