using GraphQL.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GraphQL.Persistence.Extensions;

public static class PersistenceConfigurationExtensions
{
    public static void RegisterPersistenceServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        serviceCollection.AddDbContext<DemoDbContext>(opt => opt.UseNpgsql(connectionString));
        // serviceCollection.AddDbContext<DemoDbContext>(opt => opt.UseSqlServer(connectionString)); <-- use this if mssql

    }

    public static void EnsureDatabaseCreated(this IServiceCollection serviceCollection)
    {
        using var serviceScope = serviceCollection.BuildServiceProvider().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<DemoDbContext>();
        if (context.Database.CanConnect())
        {
            // all good
            System.Console.WriteLine("All good");
        }
        
        context.Database.Migrate();
    }
}