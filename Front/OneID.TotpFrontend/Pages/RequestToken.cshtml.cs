using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

public class RequestTokenModel : PageModel
{
    [BindProperty]
    public string TotpCode { get; set; } = string.Empty;

    public string? ResultMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        using var client = new HttpClient();
        var payload = new { totpCode = TotpCode };
        var response = await client.PostAsJsonAsync("https://localhost:7200/v1/auth/request-token", payload);

        if (!response.IsSuccessStatusCode)
        {
            ResultMessage = "Código inválido ou expirado.";
            return Page();
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = json.GetProperty("token").GetString();

        Response.Cookies.Append("request_token", token!, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(2)
        });

        return Redirect("/login");
    }
}
