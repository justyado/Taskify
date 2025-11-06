using Serilog;
using Taskify.Api.Extensions;

SerilogExtensions.ConfigureSerilog();

try
{
    Log.Information("Starting Taskify API application");

    var builder = WebApplication.CreateBuilder(args);

    builder
        .AddSerilog()
        .ConfigureServices()
        .ConfigureCors()
        .AddApplicationServices();

    var app = builder.Build();

    app.ConfigureApplication();

    Log.Information("Taskify API application started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
