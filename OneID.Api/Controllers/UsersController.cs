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
            var users = new List<ActiveUserResponse>
            {
                new() { Id = "01", FullName = "Ana Ribeiro", JobTitleName = "Analista de Dados", DepartmentName = "Tecnologia", Company = "OneID Secure - SP", Login = "ana.ribeiro", Status = "Ativo", LastActivity = new DateTime(2025, 7, 25, 10, 45, 0) },
                new() { Id = "02", FullName = "Carlos Souza", JobTitleName = "DevOps", DepartmentName = "Infraestrutura", Company = "OneID Secure - RJ", Login = "carlos.souza", Status = "Inativo", LastActivity = new DateTime(2025, 6, 19, 16, 10, 0) },
                new() { Id = "03", FullName = "Fernanda Lima", JobTitleName = "Gestora de RH", DepartmentName = "Recursos Humanos", Company = "OneID Secure - SP", Login = "fernanda.lima", Status = "Ativo", LastActivity = new DateTime(2025, 7, 24, 8, 23, 0) },
                new() { Id = "04", FullName = "Marcos Vinícius", JobTitleName = "Product Owner", DepartmentName = "Produtos", Company = "OneID Secure - MG", Login = "marcos.vinicius", Status = "Ativo", LastActivity = new DateTime(2025, 7, 25, 9, 5, 0) },
                new() { Id = "05", FullName = "Juliana Martins", JobTitleName = "UX Designer", DepartmentName = "Design", Company = "OneID Secure - SP", Login = "juliana.martins", Status = "Inativo", LastActivity = new DateTime(2025, 5, 14, 17, 33, 0) },
                new() { Id = "06", FullName = "Rafael Mendes", JobTitleName = "Engenheiro de Software", DepartmentName = "Tecnologia", Company = "OneID Secure - SC", Login = "rafael.mendes", Status = "Ativo", LastActivity = new DateTime(2025, 7, 25, 11, 2, 0) },
                new() { Id = "07", FullName = "Bianca Torres", JobTitleName = "Analista de Segurança", DepartmentName = "Segurança da Informação", Company = "OneID Secure - RJ", Login = "bianca.torres", Status = "Ativo", LastActivity = new DateTime(2025, 7, 24, 20, 50, 0) },
                new() { Id = "08", FullName = "Eduardo Almeida", JobTitleName = "QA Engineer", DepartmentName = "Qualidade", Company = "OneID Secure - SP", Login = "eduardo.almeida", Status = "Inativo", LastActivity = new DateTime(2025, 4, 2, 12, 12, 0) },
                new() { Id = "09", FullName = "Larissa Gomes", JobTitleName = "Analista Financeira", DepartmentName = "Financeiro", Company = "OneID Secure - MG", Login = "larissa.gomes", Status = "Ativo", LastActivity = new DateTime(2025, 7, 25, 10, 15, 0) },
                new() { Id = "10", FullName = "Felipe Barbosa", JobTitleName = "Arquiteto de Soluções", DepartmentName = "Arquitetura", Company = "OneID Secure - SC", Login = "felipe.barbosa", Status = "Ativo", LastActivity = new DateTime(2025, 7, 25, 9, 48, 0) },
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
            public DateTime? LastActivity { get; set; }
        }
    }
}
