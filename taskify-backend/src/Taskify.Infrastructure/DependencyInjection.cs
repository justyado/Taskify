using Microsoft.Extensions.DependencyInjection;
using Taskify.Core.Interfaces.Repository;
using Taskify.Infrastructure.Database;
using Taskify.Infrastructure.Repositories;

namespace Taskify.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<DatabaseContext>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        
        return services;
    }
}