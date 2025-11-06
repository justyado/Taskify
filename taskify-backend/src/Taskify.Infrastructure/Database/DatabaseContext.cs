using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Taskify.Core.Entities;
using Taskify.Infrastructure.Database.Configurations;

namespace Taskify.Infrastructure.Database;

public class DatabaseContext(IConfiguration configuration) : DbContext
{
    public DbSet<TaskItem> TaskItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("PostgresConnection"));
        optionsBuilder.LogTo(action: Console.WriteLine, minimumLevel: LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
    }
}