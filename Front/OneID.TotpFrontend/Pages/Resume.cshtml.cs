using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class ResumeModel : PageModel
{
#nullable disable
    [BindProperty]
    public string CorrelationId { get; set; }

    public List<PendingProcessDto> PendingProcesses { get; set; } = [];
    public string ResultMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var token = Request.Cookies["access_token"];
        if (string.IsNullOrWhiteSpace(token))
        {
            ResultMessage = "Token ausente. Faça login novamente.";
            return Page();
        }

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("https://localhost:7200/v1/staging/pending");
        if (!response.IsSuccessStatusCode)
        {
            ResultMessage = "Falha ao buscar processos pendentes.";
            return Page();
        }

        var list = await response.Content.ReadFromJsonAsync<List<PendingProcessDto>>();
        PendingProcesses = list ?? [];

        return Page();
    }

    public async Task<IActionResult> OnPostResumeSingleAsync()
    {
        var token = Request.Cookies["access_token"];
        if (string.IsNullOrEmpty(token))
        {
            ResultMessage = "Token ausente.";
            return RedirectToPage();
        }

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new { CorrelationId };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://localhost:7200/v1/staging/resume", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            ResultMessage = $"Erro ao retomar: {error}";
            return RedirectToPage();
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var requestToken = json.GetProperty("token").GetString();
        var correlationId = json.GetProperty("correlationId").GetString();

        Response.Cookies.Append("correlation_id", correlationId, new CookieOptions
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
