using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApplication1.ConfigurationExtensions;

public static class StartupExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.Configure<SwaggerGeneratorOptions>(o => o.InferSecuritySchemes = true);
        builder.Services.AddScoped<IGroceryService, GroceryService>();
        builder.Services.AddDbContext<FoodContext>(o =>
        {
            o.UseInMemoryDatabase("FoodDatabase");
    
        });
        
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication("Bearer").AddJwtBearer();
        
        return builder;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        SeedDatabase(app);

        if (app.Environment.IsDevelopment())
        {
            ConfigureSwagger(app);
        }

        app.UseHttpsRedirection();

        return app;
    }

    private static void SeedDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<FoodContext>();
        context.Database.EnsureCreated();
    }

    private static void ConfigureSwagger(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}