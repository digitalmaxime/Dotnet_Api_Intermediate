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

#pragma warning disable MEAI001
public class AgentFactory
{
    public const string AgentName = "MainAgent";

    public static AIAgent CreateAgent(IServiceProvider serviceProvider, AzureOpenAiOptions azureOpenAiOptions,
        PostgresOptions postgresOptions)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

        var chatClient = new AzureOpenAIClient(
                new Uri(azureOpenAiOptions.Endpoint),
                new AzureKeyCredential(azureOpenAiOptions.ApiKey)
            ).GetChatClient(azureOpenAiOptions.DeploymentName)
            .AsIChatClient();

        var chatClientOptions = new ChatClientAgentOptions()
        {
            Name = AgentName,
            Description = "A chat agent that uses AI tools to assist users.",
            ChatOptions = new ChatOptions()
            {
                Instructions = $@"You are a utility assistant helping user username . 
                    When greeting or addressing the user, use their name: username .
                    You can get the current date/time when needed using your available tools.
                    Always provide personalized and helpful responses.",
                Tools = [AIFunctionFactory.Create(DateTimeTool.GetDateTime), PizzaDeliveryTool.ApprovalRequiredReservationTool]

            },
            ChatHistoryProviderFactory = (ctx, _) =>
            {
                var requestService = httpContextAccessor.HttpContext?.RequestServices
                                     ?? throw new InvalidOperationException("No request services found");

                ChatHistoryProvider historyProvider =
                    ActivatorUtilities.CreateInstance<MyChatMessageStore>(requestService, ctx.SerializedState,
                        postgresOptions.ConnectionString);

                return ValueTask.FromResult(historyProvider);
            }
        };

        var agent = new ChatClientAgent(chatClient, chatClientOptions)
            .AsBuilder()
            // .Use() // TODO: middleware
            .Build();

        return agent;
    }
}