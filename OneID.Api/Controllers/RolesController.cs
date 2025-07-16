using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.Commands;
using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;

namespace OneID.Api.Controllers
{
    [Route("v1/roles")]
    public class RolesController : MainController
    {
        public RolesController(ISender send) : base(send) { }

        [HttpPost]
        [Route("", Name = nameof(CreateRoleAsync))]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateRoleCommand(
                Name: request.Name,
                Description: request.Description,
                CreatedBy: User.Identity?.Name ?? "unknown"
            );

            var result = await Send.SendAsync(command, cancellationToken);

            if (result.IsSuccess)
            {
                return CreatedAtRoute(nameof(CreateRoleAsync), new
                {
                    Message = "Role criada com sucesso"
                });
            }

            return StatusCode(result.HttpCode ?? 400, new
            {
                Error = "Falha ao criar a role",
                Details = result.Message
            });
        }
    }
}
