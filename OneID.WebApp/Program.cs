using OneID.WebApp.Components;
using OneID.WebApp.Services.AuthTokens;
using OneID.WebApp.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.Information);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("OneID.ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7200/");
}).AddHttpMessageHandler<RefreshTokenHandler>();

builder.Services.AddScoped<RefreshTokenHandler>();

builder.Services.AddScoped<ITotpTokenGenerator, TotpTokenGenerator>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
