using AgentFrameworkChat.Endpoints.Conversations;
using Asp.Versioning;


namespace AgentFrameworkChat.Extensions.EndpointsExtension;

public static class ApiEndpointsExtensions
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var apiVersionedGroup = app
            .MapGroup("api/v{apiVersion:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        apiVersionedGroup.MapConversationEndpoints();
        
        app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();

        return app;
    }
}