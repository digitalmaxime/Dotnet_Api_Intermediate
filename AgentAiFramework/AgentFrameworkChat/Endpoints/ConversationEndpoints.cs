using System.Text;
using AgentFrameworkChat.AI.Agents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AgentFrameworkChat.Endpoints;

public static class ConversationEndpoints
{
    private static List<string> _conversationsHistory = new();
    
    public static IEndpointRouteBuilder MapConversationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/conversation");
        group.MapGet("", async Task<List<string>> () =>
        {
            var conversations = _conversationsHistory;
            return  conversations;
        });

        group.MapPost("", async Task<Results<Ok<string>, BadRequest<string>>> (HttpContext context, IBasicAgent agent, string message) =>
        {
            var header = context.Request.Headers["Authorization"];
            if (header.Count == 0)
            {
                return TypedResults.BadRequest("No Authorization header found");
            }

            // read the token from the header
            var accessToken = header[0]?.Replace("Bearer ", "");
            var handler = new JsonWebTokenHandler();
            if (!handler.CanReadToken(accessToken)) return TypedResults.BadRequest("Invalid token");
            var token = handler.ReadJsonWebToken(accessToken);
            var username = token.Claims.First(c => c.Type == "name").Value;
            _conversationsHistory.Add(message);  // TODO: save to DB
            var response = await agent.SendMessage(message, username);
            return TypedResults.Ok(response);
        });
        
        return endpoints;
    }
}