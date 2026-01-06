using System.Text;
using AgentFrameworkChat.AI;

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

        group.MapPost("", async Task<string> (IBasicAgent agent, string message) =>
        {
            _conversationsHistory.Add(message);
            var response = await agent.SendMessage(message);
            return response;
        });
        
        return endpoints;
    }
}