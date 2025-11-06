using Microsoft.EntityFrameworkCore;
using Serilog;
using Taskify.Api.Middlewares;
using Taskify.Infrastructure.Database;

namespace Taskify.Api.Extensions;

public static class ApplicationExtensions
{
    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        // Автоматическое применение миграций при запуске
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            Log.Information("Applying database migrations...");
            dbContext.Database.Migrate();
            Log.Information("Database migrations applied successfully");
        }

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseSerilogRequestLogging();
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapOpenApi();
        }

        app.UseCors("AllowFrontend");
        app.UseHttpsRedirection();
        app.MapControllers();

        return app;
    }
}
