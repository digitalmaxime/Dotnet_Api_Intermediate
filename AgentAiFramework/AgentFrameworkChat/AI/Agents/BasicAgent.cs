using System.Text.Json;
using AgentFrameworkChat.AI.History;
using AgentFrameworkChat.AI.Tools;
using AgentFrameworkChat.Options;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;


namespace AgentFrameworkChat.AI.Agents;

public class BasicAgent(
    IOptions<AzureOpenAiOptions> azureOpenAiConfiguration,
    IOptions<PostgresOptions> postgresOptions,
    IThreadStore threadStore)
    : IBasicAgent
{
    public async Task<string> SendMessage(string message, string username)
    {
        
        var reservationTool = AIFunctionFactory.Create(PizzaDeliveryTool.OrderPizza);
#pragma warning disable MEAI001
        var approvalRequiredReservationTool = new ApprovalRequiredAIFunction(reservationTool);
        var agent = new AzureOpenAIClient(
                new Uri(azureOpenAiConfiguration.Value.endpoint),
                new AzureKeyCredential(azureOpenAiConfiguration.Value.apiKey)
            )
            .GetChatClient(azureOpenAiConfiguration.Value.deploymentName)
            .CreateAIAgent(new ChatClientAgentOptions
            {
                Name = "UtilityToolAgent",
                Description = "A chat agent that uses AI tools to assist users.",
                ChatOptions = new ChatOptions()
                {
                    Instructions = $@"You are a utility assistant helping user '{username}'. 
                    When greeting or addressing the user, use their name: {username}.
                    You can get the current date/time when needed using your available tools.
                    Always provide personalized and helpful responses.",
                    Tools = [AIFunctionFactory.Create(DateTimeTool.GetDateTime), approvalRequiredReservationTool]
                    // ResponseFormat = new ChatResponseFormatJson(J) // TODO:
                },
                ChatMessageStoreFactory = ctx => new MyChatMessageStore(
                    ctx.SerializedState,
                    postgresOptions.Value.ConnectionString, username)
            });
        
        var savedState = await threadStore.LoadThreadStateForUserAsync(username);

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
    }
    
}