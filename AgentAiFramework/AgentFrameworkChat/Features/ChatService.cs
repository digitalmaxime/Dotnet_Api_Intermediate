using System.Security.Claims;
using System.Text.Json;
using AgentFrameworkChat.AI.Agents;
using AgentFrameworkChat.AI.History;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkChat.Features;

public interface IChatService
{
    Task<string> SendMessage(string conversationId, string message);
}

public class ChatService(IAgentFactory agentFactory, IThreadStore threadStore) : IChatService
{
    public async Task<string> SendMessage(string conversationId, string message)
    {
        var savedState = await threadStore.LoadThreadStateForUserAsync(conversationId);

        var agent = agentFactory.CreateAgent();
        
        AgentThread thread;
        if (savedState is not null)
        {
            thread = agent.DeserializeThread(savedState.Value);
            // TODO: validate user can access this conversation
            Console.WriteLine("Loaded thread state from database");
        }
        else
        {
            thread = agent.GetNewThread(); // First‑ever conversation for this user
            JsonElement serializedThreadState = thread.Serialize();
            await threadStore.SaveThreadStateForUserAsync(conversationId, serializedThreadState);
            Console.WriteLine("Saved thread state to database");
        }
        
#pragma warning disable MEAI001
        var response = await agent.RunAsync(message, thread);
        // var functionApprovalRequests = response.Messages
        //     .SelectMany(x => x.Contents)
        //     .OfType<FunctionApprovalRequestContent>()
        //     .ToList();
        //
        // FunctionApprovalRequestContent requestContent = functionApprovalRequests.First();
        // Console.WriteLine($"We require approval to execute '{requestContent.FunctionCall.Name}'");
        // var approvalMessage = new ChatMessage(ChatRole.User, [requestContent.CreateResponse(true)]);
        // Console.WriteLine(await agent.RunAsync(approvalMessage, thread));

        return response.Text;
        agent.RunAsync(message, thread);
        return "Hello World";
    }
}