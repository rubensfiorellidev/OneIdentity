using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class ConfirmModel : PageModel
{
    [BindProperty]
    public string TotpCode { get; set; } = string.Empty;

    [BindProperty]
    public string CorrelationId { get; set; } = string.Empty;

    public string? ResultMessage { get; set; }

    public string OperatorName { get; private set; } = "usuário";

    public void OnGet()
    {
        var token = Request.Cookies["access_token"];

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "name");
                OperatorName = nameClaim?.Value ?? "usuário";
            }
            catch
            {
                OperatorName = "usuário";
            }
        }
    }
    public async Task<IActionResult> OnPostAsync()
    {
        var token = Request.Cookies["access_token"];
        if (string.IsNullOrEmpty(token))
        {
            ResultMessage = "Token ausente. Faça login novamente.";
            return Page();
        }

        var payload = new { totpCode = TotpCode, correlationId = CorrelationId };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var httpClient = new HttpClient(handler);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.PostAsync("https://localhost:7200/v1/staging/confirm", content);

        ResultMessage = response.IsSuccessStatusCode
            ? "Código confirmado! Provisionamento iniciado."
            : "Erro: " + await response.Content.ReadAsStringAsync();

        try
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwt = jwtHandler.ReadJwtToken(token);
            var nameClaim = jwt.Claims.FirstOrDefault(c => c.Type == "name");
            OperatorName = nameClaim?.Value ?? "usuário";
        }
        catch
        {
            OperatorName = "usuário";
        }

        return Page();
    }
}
