using OneID.WebApp.Components;
using OneID.WebApp.Interfaces;
using OneID.WebApp.Services.ActiveUsers;
using OneID.WebApp.Services.AuthTokens;
using OneID.WebApp.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AccessTokenHandler>();
builder.Services.AddScoped<RefreshTokenHandler>();

builder.Services.AddHttpClient<IOneIdUserService, OneIdUserService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7200/");
})
.AddHttpMessageHandler<AccessTokenHandler>()
.AddHttpMessageHandler<RefreshTokenHandler>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<AccessTokenHandler>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "access_token";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });


builder.Services.AddHttpClient("AuthenticatedClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7200/");

}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    UseCookies = true

}).AddHttpMessageHandler<RefreshTokenHandler>();

builder.Services.AddScoped<RefreshTokenHandler>();
builder.Services.AddScoped<ITotpTokenGenerator, TotpTokenGenerator>();
builder.Services.AddScoped<IOneIdUserService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("AuthenticatedClient");
    return new OneIdUserService(client);
});


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
