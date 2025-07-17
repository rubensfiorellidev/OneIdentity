using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {

        var token = Request.Cookies["request_token"];
        if (string.IsNullOrEmpty(token))
        {
            ErrorMessage = "Token de requisição ausente. Volte e confirme o TOTP.";
            return Redirect("/request-token");
        }

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new { login = Username, password = Password };
        var response = await client.PostAsJsonAsync("https://localhost:7200/v1/auth/login", payload);


        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Login inválido.";
            return Page();
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        if (!json.TryGetProperty("token", out var jwtoken))
        {
            ErrorMessage = "Token JWT não retornado pela API.";
            return Page();
        }
        var accessToken = json.GetProperty("token").GetString();
        //var refreshToken = json.GetProperty("refreshToken").GetString();


        Response.Cookies.Append("access_token", accessToken!, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        });

        return Redirect("/dashboard");
    }
}
