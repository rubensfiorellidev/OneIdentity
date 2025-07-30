using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Http.Headers;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var requestToken = Request.Cookies["request_token"];

        if (string.IsNullOrEmpty(requestToken))
        {
            ErrorMessage = "Token de requisição ausente. Volte e confirme o TOTP.";
            return Redirect("/request-token");
        }

        using var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };

        using var client = new HttpClient(handler);
        var payload = new { login = Username, password = Password };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestToken);
        client.DefaultRequestHeaders.UserAgent.ParseAdd(Request.Headers.UserAgent.ToString());

        var response = await client.PostAsJsonAsync("https://localhost:7200/v1/auth/login", payload);

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Login inválido.";
            return Page();
        }

        foreach (var cookie in handler.CookieContainer.GetCookies(new Uri("https://localhost:7200")).Cast<Cookie>())
        {
            Response.Cookies.Append(cookie.Name, cookie.Value, new CookieOptions
            {
                HttpOnly = cookie.HttpOnly,
                Secure = cookie.Secure,
                SameSite = SameSiteMode.Strict,
                Expires = cookie.Expires != DateTime.MinValue ? cookie.Expires : DateTimeOffset.UtcNow.AddHours(1)
            });
        }

        return Redirect("https://localhost:5002/dashboard");
    }

}
