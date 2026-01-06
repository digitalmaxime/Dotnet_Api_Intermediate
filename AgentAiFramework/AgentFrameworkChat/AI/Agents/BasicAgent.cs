using AgentFrameworkChat.AI.History;
using AgentFrameworkChat.AI.Tools;
using AgentFrameworkChat.Options;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.PgVector;
using OpenAI;
using OpenAI.Assistants;

namespace AgentFrameworkChat.AI.Agents;

public class BasicAgent(
    IOptions<AzureOpenAiOptions> azureOpenAiConfiguration,
    IOptions<PostgresOptions> postgresOptions)
    : IBasicAgent
{
    // Simple in-memory cache of threads per user
    private static readonly Dictionary<string, AgentThread> _threadCache = new();
    public async Task<string> SendMessage(string message, string username)
    {
        AIAgent agent = new AzureOpenAIClient(
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
                    Tools = [AIFunctionFactory.Create(DateTimeTool.GetDateTime)]
                },
                ChatMessageStoreFactory = ctx => new MyChatMessageStore(ctx.SerializedState, postgresOptions.Value.ConnectionString, username)
            });
        
        // Get or create a thread for this user
        if (!_threadCache.TryGetValue(username, out var thread))
        {
            thread = agent.GetNewThread();
            _threadCache[username] = thread;
        }        
        var response = await agent.RunAsync(message, thread);
        
        return response.Text;
    }
}