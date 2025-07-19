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
        if (string.IsNullOrWhiteSpace(CorrelationId))
            CorrelationId = Request.Cookies["correlation_id"] ?? string.Empty;

        SetOperatorNameFromJwt();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(TotpCode) || string.IsNullOrWhiteSpace(CorrelationId))
        {
            ResultMessage = "Código ou ID de correlação ausente.";
            SetOperatorNameFromJwt();
            return Page();
        }

        var token = Request.Cookies["request_token"];
        if (string.IsNullOrWhiteSpace(token))
        {
            ResultMessage = "Token de requisição ausente. Refaça o processo.";
            return Page();
        }

        var payload = new
        {
            correlationId = CorrelationId,
            totp = TotpCode
        };

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var httpClient = new HttpClient(handler);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("https://localhost:7200/v1/staging/confirm", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            ResultMessage = "Código confirmado! Provisionamento iniciado.";
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            ResultMessage = $"Erro: {error}";
        }

        SetOperatorNameFromJwt(token);
        return Page();
    }

    private void SetOperatorNameFromJwt(string? token = null)
    {
        token ??= Request.Cookies["access_token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            OperatorName = "usuário";
            return;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            OperatorName = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "usuário";
        }
        catch
        {
            OperatorName = "usuário";
        }
    }
}
