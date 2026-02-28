using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace AgentFrameworkChat.Extensions.OpenApi;

public static class OpenApiExtension
{
    public static IServiceCollection ConfigureOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
        
        services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

        return services;
    }

    public static WebApplication UseOpenApiDocumentation(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
            options.AddDocument("v1", "Api Documentation version 1.0.0");
            options.AddPreferredSecuritySchemes(JwtBearerDefaults.AuthenticationScheme);
            options.Authentication = new ScalarAuthenticationOptions
            {
                PreferredSecurityScheme = JwtBearerDefaults.AuthenticationScheme,
                
                // Optional: prefill a token during development
                // Live token input is handled by the transformer below
            };
        });
        
        // app.MapScalarApiReference(options => options
        //     .AddPreferredSecuritySchemes("BearerAuth")
        //     .AddHttpAuthentication("BearerAuth", auth =>
        //     {
        //         auth.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
        //     }));

        return app;
    }

    #region OpenApi Transformer
    internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            };

            document.Components ??= new OpenApiComponents();

            document.AddComponent("Bearer", bearerScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            };

            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations!.Values))
            {
                operation.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Security.Add(securityRequirement);
            }

            return Task.CompletedTask;
        }
    } 
#endregion
}
