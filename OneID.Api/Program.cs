using OneID.Application;
using OneID.Data;
using OneID.Domain.Abstractions.MiddlewareTracings;
using OneID.Messaging;
using OneID.Shared;
using OneID.Shared.Tools;
using Serilog;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

Log.Logger = new LoggerConfiguration()
    .Enrich.With<ActivityEnricher>()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [TraceId:{TraceId} SpanId:{SpanId}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

var certificateConfig = builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate");

var passwordValue = certificateConfig["Password"];
var certPassword = passwordValue != null && passwordValue.StartsWith("ENV:")
    ? Environment.GetEnvironmentVariable(passwordValue.Replace("ENV:", ""))
    : passwordValue;
certPassword ??= string.Empty;
builder.Configuration["Kestrel:Endpoints:Https:Certificate:Password"] = certPassword;

builder.WebHost.ConfigureKestrel(options =>
{
    options.Configure(builder.Configuration.GetSection("Kestrel"));
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


builder.Services.AddAuthorizationBuilder().SetFallbackPolicy(null);

builder.Services
    .AddData(builder.Configuration, builder.Environment)
    .AddInfrastructure(builder.Configuration, builder.Environment)
    .AddApplication(builder.Configuration)
    .AddMassTrasitConfig()
    .AddRabbitSetup(builder.Configuration, builder.Environment)
    .AddJwtAuthentication(builder.Configuration)
    .AddApiPipelineConfiguration()
    .AddRedis(builder.Configuration)
    .AddOpenTelemetry(builder.Configuration, builder.Environment);


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

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.ParseStateValues = true;
    options.IncludeFormattedMessage = true;
});

var app = builder.Build();

var environmentName = app.Environment.EnvironmentName;
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<Program>();

logger.LogInformation("Application is running in {Environment} environment.", environmentName);
logger.LogInformation("Application is running in {TimeZone} TimeZone.", TimeZoneInfo.Local.Id);
logger.LogInformation("Application started at {Now} (Local).", DateTimeOffset.Now);
logger.LogInformation("Application started at {UtcNow} (UTC).", DateTimeOffset.UtcNow);

app.UseSerilogRequestLogging();
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevCookieCors");
}
else
{
    app.UseCors("OneIdBackendCustom");
}

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
app.UseMiddleware<TracingMiddleware>();
app.MapControllers();

app.Run();
Log.CloseAndFlush();