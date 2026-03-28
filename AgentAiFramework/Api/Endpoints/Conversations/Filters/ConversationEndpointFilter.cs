namespace AgentFrameworkChat.Endpoints.Conversations.Filters;

public class ConversationEndpointFilter: IEndpointFilter
{
    private static readonly string[] MissingNameClaim = ["Username not found in claims"];
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var username = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "name")?.Value;

        if (!string.IsNullOrEmpty(username))
        {
            return await next(context);
        }

        var details = new Dictionary<string, string[]>()
        {
            { "content", MissingNameClaim }
        };
        
        return TypedResults.ValidationProblem(details);
    }
}