using System.Text.Json;
using Microsoft.Extensions.VectorData;

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
using Npgsql; 

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
                ChatMessageStoreFactory = ctx => new MyChatMessageStore(
                    ctx.SerializedState, 
                    postgresOptions.Value.ConnectionString, username)
            });
        
        var savedState = await LoadThreadStateForUserAsync(username);

        AgentThread thread;
        if (savedState is not null)
        {
            // Restore existing thread with its internal state + reference to your ChatMessageStore
            thread = agent.DeserializeThread(savedState ?? throw new InvalidOperationException()); 
        }
        else
        {
            // First‑ever conversation for this user
            thread = agent.GetNewThread();
            // Serialize the thread state to a JsonElement, so it can be stored for later use.
            JsonElement serializedThreadState = thread.Serialize();
            await SaveThreadStateForUserAsync(username, serializedThreadState);
        }
        
        var response = await agent.RunAsync(message, thread);
        
        return response.Text;
    }
    
    
    
    public class ThreadStateSchema
    {
        // This will be the primary key (e.g. username)
        [VectorStoreKey]
        public string Key { get; set; } = default!;

        // Your serialized AgentThread state as JSON text
        [VectorStoreData]
        public string SerializedState { get; set; } = default!;
    }
    
    private async Task SaveThreadStateForUserAsync(string username, JsonElement state, CancellationToken cancellationToken = default)
    {
        // Reuse the same PostgresVectorStore connection string you already use
        var vectorStore = new PostgresVectorStore(postgresOptions.Value.ConnectionString);

        var collection = vectorStore.GetCollection<string, ThreadStateSchema>("ThreadState");
        await collection.EnsureCollectionExistsAsync(cancellationToken);

        var jsonText = state.Clone().GetRawText();

        var record = new ThreadStateSchema
        {
            Key = username,
            SerializedState = jsonText
        };

        await collection.UpsertAsync(new[] { record }, cancellationToken);
    }
    
    private async Task<JsonElement?> LoadThreadStateForUserAsync(string username, CancellationToken cancellationToken = default)
    {
        var vectorStore = new PostgresVectorStore(postgresOptions.Value.ConnectionString);

        var collection = vectorStore.GetCollection<string, ThreadStateSchema>("ThreadState");
        await collection.EnsureCollectionExistsAsync(cancellationToken);

        await foreach (var record in collection.GetAsync(r => r.Key == username, top: 1, cancellationToken: cancellationToken))
        {
            using var doc = JsonDocument.Parse(record.SerializedState);
            return doc.RootElement.Clone();
        }

        return null;
    }
}