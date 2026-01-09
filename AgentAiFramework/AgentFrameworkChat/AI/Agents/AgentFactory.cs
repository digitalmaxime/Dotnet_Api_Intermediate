using System.Security.Claims;
using AgentFrameworkChat.AI.History;
using AgentFrameworkChat.AI.Tools;
using AgentFrameworkChat.Options;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace AgentFrameworkChat.AI.Agents;

public interface IAgentFactory
{
    AIAgent CreateAgent();
}

public class AgentFactory(
    IOptions<AzureOpenAiOptions> azureOpenAiConfiguration,
    IOptions<PostgresOptions> postgresOptions,
    IHttpContextAccessor httpContextAccessor) : IAgentFactory
{
    public AIAgent CreateAgent()
    {
        var user = httpContextAccessor.HttpContext?.User;

        var username =
            user?.FindFirstValue(ClaimTypes.Name) ??
            user?.FindFirstValue("name") ??
            "anonymous";

        var reservationTool = AIFunctionFactory.Create(ReservationTool.MakeAReservation);
#pragma warning disable MEAI001
        var approvalRequiredReservationTool = new ApprovalRequiredAIFunction(reservationTool);

        var chatClient = new AzureOpenAIClient(
                new Uri(azureOpenAiConfiguration.Value.endpoint),
                new AzureKeyCredential(azureOpenAiConfiguration.Value.apiKey)
            ).GetChatClient(azureOpenAiConfiguration.Value.deploymentName);
        
        return chatClient
            .CreateAIAgent(new ChatClientAgentOptions
            {
                Name = "BasicChatAgent",
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
                    ctx.SerializedState, postgresOptions.Value.ConnectionString, username)
            });
    }
}