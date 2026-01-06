using AgentFrameworkChat.AI.Tools;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

namespace AgentFrameworkChat.AI;

public class BasicAgent : IBasicAgent
{
    private readonly string endpoint = "";
    private readonly string deploymentName = "";
    private readonly string apiKey = "";

    // private readonly AIAgent agent;

    public BasicAgent()
    {
    }

    public async Task<string> SendMessage(string message)
    {
        AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .CreateAIAgent(
                name: "UtilityToolAgent",
                instructions:
                "You are a utility assistant that can get the current date/time. When asked for this information, use your available tools.",
                description: "An agent that can get the current date/time.",
                tools: [AIFunctionFactory.Create(DateTimeTool.GetDateTime)]
            );
        
        var response = await agent.RunAsync("Tell me a what day it is in a surprising way");
        return response.Text;
    }
}