using GraphQL.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.Persistence.Extensions;

public static class PersistenceConfigurationExtensions
{
    public static void RegisterPersistenceServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        serviceCollection.AddDbContext<DemoDbContext>(opt => opt.UseSqlServer(connectionString));
   
    }
    
    public static void EnsureDatabaseCreated(this IServiceCollection serviceCollection)
    {
        using var serviceScope = serviceCollection.BuildServiceProvider().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<DemoDbContext>();
        context.Database.Migrate();
        
        // using var sp = serviceCollection.BuildServiceProvider();
        // var context = sp.GetRequiredService<DemoDbContext>();
        // context.Database.Migrate();
    }
}