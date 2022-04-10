using Ocelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile($"./Configuration/ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
});

//builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();

string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm: ss.fff} [{Level}] { Message} { NewLine} { Exception}";

// Add services to the container.
var logger = new LoggerConfiguration()
    .WriteTo.Debug()
    .WriteTo.Console()
    //.WriteTo.File("./Logs/Demo.txt", rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());

var app = builder.Build();

await app.UseOcelot();

app.Run();
