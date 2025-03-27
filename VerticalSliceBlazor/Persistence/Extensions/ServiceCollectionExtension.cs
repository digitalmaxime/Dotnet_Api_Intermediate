using Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Database;
using Persistence.Repositories;

namespace Persistence.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(opt =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            // opt.UseSqlite(connectionString);
            opt.UseNpgsql(connectionString);
        });

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}