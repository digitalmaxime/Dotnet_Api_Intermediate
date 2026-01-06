using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;

namespace AgentFrameworkChat.Extensions.OpenApi;

public static class OpenApiExtension
{
    public static void ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(config =>
            {
                // Add security definition
                var scheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                };

                config.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new List<string>()
                    }
                });

                config.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            })
            .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureScalarOptions>();
    }
    
    public static void UseOpenApiDocumentation(this WebApplication app)
    {
        app.UseSwagger(opt => opt.RouteTemplate = "openapi/{documentName}.json");
        app.MapScalarApiReference();
    }
}