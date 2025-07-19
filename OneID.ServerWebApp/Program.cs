using OneID.ServerWebApp.Components;
using OneID.ServerWebApp.Shared.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
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

app.MapRazorPages();

app.MapGet("/request-token", context =>
{
    context.Response.Redirect("/Auth/RequestToken");
    return Task.CompletedTask;
});


app.Run();
