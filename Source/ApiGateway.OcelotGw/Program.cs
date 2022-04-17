using Ocelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.Middleware;
using Serilog;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile($"./Configuration/ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
});

// Add services to the container.
var logger = new LoggerConfiguration()
    .WriteTo.Debug()
    .WriteTo.Console()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());

var app = builder.Build();

await app.UseOcelot();

app.Run();
