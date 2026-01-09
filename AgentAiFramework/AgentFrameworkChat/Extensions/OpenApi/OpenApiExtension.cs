using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

namespace AgentFrameworkChat.Extensions.OpenApi;

public static class OpenApiExtension
{
    public static void ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi();
    }

    public static void UseOpenApiDocumentation(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options => options
            .AddPreferredSecuritySchemes("BearerAuth")
            .AddHttpAuthentication("BearerAuth", auth =>
            {
                auth.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
            }));
    }
}