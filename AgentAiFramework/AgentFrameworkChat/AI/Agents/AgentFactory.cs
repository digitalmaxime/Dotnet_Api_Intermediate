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


public class AgentFactory
{
    public static AIAgent CreateAgent(IServiceProvider serviceProvider, AzureOpenAiOptions azureOpenAiOptions, PostgresOptions postgresOptions)
    {

        var pizzaOrderTool = AIFunctionFactory.Create(PizzaDeliveryTool.OrderPizza);
#pragma warning disable MEAI001
        var approvalRequiredReservationTool = new ApprovalRequiredAIFunction(pizzaOrderTool);

        
        
        var chatClient = new AzureOpenAIClient(
                new Uri(azureOpenAiOptions.Endpoint),
                new AzureKeyCredential(azureOpenAiOptions.ApiKey)
            ).GetChatClient(azureOpenAiOptions.DeploymentName);
        
        return chatClient
            .CreateAIAgent(new ChatClientAgentOptions
            {
                Name = "BasicChatAgent",
                Description = "A chat agent that uses AI tools to assist users.",
                ChatOptions = new ChatOptions()
                {
                    Instructions = $@"You are a utility assistant helping user username . 
                    When greeting or addressing the user, use their name: username .
                    You can get the current date/time when needed using your available tools.
                    Always provide personalized and helpful responses.",
                    Tools = [AIFunctionFactory.Create(DateTimeTool.GetDateTime), approvalRequiredReservationTool]
                    // ResponseFormat = new ChatResponseFormatJson(J) // TODO:
                },
                ChatMessageStoreFactory = ctx => new MyChatMessageStore(
                    ctx.SerializedState, postgresOptions.ConnectionString, "username")
            }, services: serviceProvider);
    }
}