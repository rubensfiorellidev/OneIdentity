using Microsoft.AspNetCore.Mvc;
using OneID.Application.Interfaces.CQRS;

namespace OneID.Api.Controllers
{
    [Route("v1/users")]
    public class UsersController : MainController
    {
        public UsersController(ISender send) : base(send)
        {
        }

        [HttpGet("active-users")]
        public IActionResult GetActiveUsers()
        {
            // Simulando usuários mockados — você pode trocar isso por um repositório/serviço real
            var users = new List<ActiveUserResponse>
        {
            new() { Id = "01", FullName = "Ana Ribeiro", JobTitleName = "Analista de Dados", DepartmentName = "Tecnologia", Company = "OneID Secure", Login = "ana.ribeiro", Status = "Ativo" },
            new() { Id = "02", FullName = "Carlos Souza", JobTitleName = "DevOps", DepartmentName = "Infraestrutura", Company = "OneID Secure", Login = "carlos.souza", Status = "Ativo" },
            new() { Id = "03", FullName = "Fernanda Lima", JobTitleName = "Gestora de RH", DepartmentName = "RH", Company = "OneID Secure", Login = "fernanda.lima", Status = "Ativo" },
        };

            return Ok(users);
        }

        public class ActiveUserResponse
        {
            public string Id { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string JobTitleName { get; set; } = string.Empty;
            public string DepartmentName { get; set; } = string.Empty;
            public string Company { get; set; } = string.Empty;
            public string Login { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }
    }
}
