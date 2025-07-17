using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;

public class DashboardModel : PageModel
{
    public string OperatorName { get; set; } = "usu�rio";
    public string Email { get; set; } = "desconhecido";
    public List<string>? Roles { get; set; }

    public void OnGet()
    {
        var token = Request.Cookies["access_token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            OperatorName = "usu�rio n�o autenticado";
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var jwt = handler.ReadJwtToken(token);
            OperatorName = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "usu�rio";
            Email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "n�o informado";

            var rolesClaim = jwt.Claims.FirstOrDefault(c => c.Type == "roles")?.Value;
            if (!string.IsNullOrEmpty(rolesClaim))
            {
                Roles = System.Text.Json.JsonSerializer.Deserialize<List<string>>(rolesClaim);
            }
        }
        catch
        {
            OperatorName = "erro ao ler token";
        }
    }
}
