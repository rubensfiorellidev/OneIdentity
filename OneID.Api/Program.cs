using Microsoft.AspNetCore.Server.Kestrel.Core;
using OneID.Application;
using OneID.Data;
using OneID.Messaging;
using OneID.Shared;
using OneID.Shared.Tools;
using Serilog;
using System.IO.Compression;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var certPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".aspnet", "https", "OneID.Api.pfx");

    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ServerCertificate = new X509Certificate2(certPath);
        httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
    });

    serverOptions.ListenAnyIP(7200, listenOptions =>
    {
        listenOptions.UseHttps();
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
    });

});

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder.Host.UseSerilog((context, services, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .ReadFrom.Services(services);
});

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog();
});


builder.Services.AddData(builder.Configuration, builder.Environment);
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddMassTrasitConfig();
builder.Services.AddRabbitSetup(builder.Configuration, builder.Environment);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationBuilder().SetFallbackPolicy(null);
builder.Services.AddApiPipelineConfiguration();


builder.Services.AddControllers(options =>
{
    options.Filters.Add<SanitizeInputFilter>();
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new TypeUserAccountNewtonsoftConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("CloudFrontClient", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
});

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
});



var app = builder.Build();

var environmentName = app.Environment.EnvironmentName;
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<Program>();

logger.LogInformation("Application is running in {Environment} environment.", environmentName);
logger.LogInformation("Application is running in {TimeZone} TimeZone.", TimeZoneInfo.Local.Id);
logger.LogInformation("Application started at {Now} (Local).", DateTime.Now);
logger.LogInformation("Application started at {UtcNow} (UTC).", DateTime.UtcNow);

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (builder.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
}
else
{
    app.UseCors("OneIdBackendCustom");
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}


app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Post &&
        context.Request.Headers.TryGetValue("Content-Encoding", out var encoding) &&
        encoding.ToString().Contains("gzip", StringComparison.OrdinalIgnoreCase))
    {
        using var decompressedBody = new MemoryStream();
        using var gzipStream = new GZipStream(context.Request.Body, CompressionMode.Decompress);
        await gzipStream.CopyToAsync(decompressedBody);
        decompressedBody.Seek(0, SeekOrigin.Begin);
        context.Request.Body = decompressedBody;
    }

    await next();
});

app.UseResponseCompression();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();
Log.CloseAndFlush();