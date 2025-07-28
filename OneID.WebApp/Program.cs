using Microsoft.AspNetCore.Components.Server.Circuits;
using OneID.WebApp.Components;
using OneID.WebApp.Interfaces;
using OneID.WebApp.Services;
using OneID.WebApp.Services.ActiveUsers;
using OneID.WebApp.Services.AuthTokens;
using OneID.WebApp.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Handlers que cuidam dos tokens
builder.Services.AddScoped<AccessTokenHandler>();
builder.Services.AddScoped<RefreshTokenHandler>();

// Circuito que renova o token periodicamente
builder.Services.AddSingleton<CircuitHandler, TokenCircuitHandler>();

// HttpClient principal com handlers aplicados
builder.Services.AddHttpClient<IOneIdUserService, OneIdUserService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7200/");
})
.AddHttpMessageHandler<AccessTokenHandler>();
//.AddHttpMessageHandler<RefreshTokenHandler>();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "access_token";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITotpTokenGenerator, TotpTokenGenerator>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
