using System.Security.Claims;
using System.Text.Json;
using AgentFrameworkChat.AI.Agents;
using AgentFrameworkChat.AI.History;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkChat.Features;

public interface IChatService
{
    Task<string> SendMessage(ClaimsPrincipal user, string message);
}

public class ChatService(IAgentFactory agentFactory, IThreadStore threadStore) : IChatService
{
    public async Task<string> SendMessage(ClaimsPrincipal user, string message)
    {
        var username = user.FindFirstValue(ClaimTypes.Name) ??
                       user.FindFirstValue("name") ??
                       "anonymous";

        var savedState = await threadStore.LoadThreadStateForUserAsync(username);

        var agent = agentFactory.CreateAgent();
        
        AgentThread thread;
        if (savedState is not null)
        {
            // Restore existing thread with its internal state + reference to your ChatMessageStore
            thread = agent.DeserializeThread(savedState.Value);
            Console.WriteLine("Loaded thread state from database");
        }
        else
        {
            thread = agent.GetNewThread(); // First‑ever conversation for this user
            // Serialize the thread state to a JsonElement, so it can be stored for later use.
            JsonElement serializedThreadState = thread.Serialize();
            await threadStore.SaveThreadStateForUserAsync(username, serializedThreadState);
            Console.WriteLine("Saved thread state to database");
        }
        
#pragma warning disable MEAI001
        var response = await agent.RunAsync(message, thread);
        var functionApprovalRequests = response.Messages
            .SelectMany(x => x.Contents)
            .OfType<FunctionApprovalRequestContent>()
            .ToList();

        FunctionApprovalRequestContent requestContent = functionApprovalRequests.First();
        Console.WriteLine($"We require approval to execute '{requestContent.FunctionCall.Name}'");
        var approvalMessage = new ChatMessage(ChatRole.User, [requestContent.CreateResponse(true)]);
        Console.WriteLine(await agent.RunAsync(approvalMessage, thread));

        return response.Text;
        agent.RunAsync(message, thread);
        return "Hello World";
    }
}