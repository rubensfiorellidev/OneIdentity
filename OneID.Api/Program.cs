using OneID.Application;
using OneID.Data;
using OneID.Messaging;
using OneID.Shared;
using OneID.Shared.Tools;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;

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


app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCompression();
app.UseRateLimiter();


app.MapControllers();

app.Run();
Log.CloseAndFlush();