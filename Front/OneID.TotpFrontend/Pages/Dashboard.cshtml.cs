using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

#nullable disable
public class DashboardModel : PageModel
{
    public string OperatorName { get; set; } = "usuário";
    public string Email { get; set; } = "desconhecido";
    public List<string> Roles { get; set; }
    public List<PendingProcessDto> PendingProcesses { get; set; } = [];
    public string AccountId { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var expand = Request.Query.ContainsKey("expand");

        var token = Request.Cookies["access_token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            OperatorName = "usuário não autenticado";
            return Page();
        }

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var jwt = handler.ReadJwtToken(token);
            OperatorName = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "usuário";
            Email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "não informado";
            AccountId = jwt.Claims.FirstOrDefault(c => c.Type == "account_id")?.Value ?? "desconhecido";

            var rolesClaim = jwt.Claims.FirstOrDefault(c => c.Type == "roles")?.Value;
            if (!string.IsNullOrEmpty(rolesClaim))
            {
                Roles = JsonSerializer.Deserialize<List<string>>(rolesClaim);
            }
        }
        catch
        {
            OperatorName = "erro ao ler token";
        }

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("https://localhost:7200/v1/pendings");
        if (response.IsSuccessStatusCode)
        {
            var list = await response.Content.ReadFromJsonAsync<List<PendingProcessDto>>();
            PendingProcesses = list ?? [];
        }

        ViewData["ExpandGrid"] = expand;

        return Page();
    }

    public async Task<IActionResult> OnPostResumeAsync(string correlationId)
    {
        var token = Request.Cookies["access_token"];
        if (string.IsNullOrWhiteSpace(token))
            return RedirectToPage("/login");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new { CorrelationId = correlationId };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://localhost:7200/v1/staging/resume", content);

        if (!response.IsSuccessStatusCode)
        {
            // TODO: exibir erro opcionalmente
            return RedirectToPage();
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var requestToken = json.GetProperty("token").GetString();
        var resumedCorrelationId = json.GetProperty("correlationId").GetString();

        Response.Cookies.Append("correlation_id", resumedCorrelationId, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(5)
        });

        Response.Cookies.Append("request_token", requestToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(5)
        });

        return Redirect("/confirm-code");
    }

    public class PendingProcessDto
    {
        public string CorrelationId { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
