using System.Text.Json;
using AgentFrameworkChat.Options;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Connectors.PgVector;
using Microsoft.Extensions.VectorData;
namespace AgentFrameworkChat.AI.History;

public interface IThreadStore
{
    public Task SaveThreadStateForUserAsync(string username, JsonElement state, CancellationToken cancellationToken = default);
    public Task<JsonElement?> LoadThreadStateForUserAsync(string username, CancellationToken cancellationToken = default);
}

public sealed class ThreadStore( IOptions<PostgresOptions> postgresOptions) : IThreadStore
{
    public async Task SaveThreadStateForUserAsync(string username, JsonElement state, CancellationToken cancellationToken = default)
    {
        // Reuse the same PostgresVectorStore connection string you already use
        var vectorStore = new PostgresVectorStore(postgresOptions.Value.ConnectionString);
        var collection = vectorStore.GetCollection<string, ThreadStateSchema>("ThreadState");
        await collection.EnsureCollectionExistsAsync(cancellationToken);

        var jsonText = state.Clone().GetRawText();

        var record = new ThreadStateSchema
        {
            Key = VectorStoreHelper.GenerateIntKeyFromString(username),
            SerializedState = jsonText
        };

        await collection.UpsertAsync(new[] { record }, cancellationToken);
    }
    
    public async Task<JsonElement?> LoadThreadStateForUserAsync(string username, CancellationToken cancellationToken = default)
    {
        var vectorStore = new PostgresVectorStore(postgresOptions.Value.ConnectionString);

        var collection = vectorStore.GetCollection<string, ThreadStateSchema>("ThreadState");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        var hashedKey = VectorStoreHelper.GenerateIntKeyFromString(username);
        await foreach (var record in collection.GetAsync(r => r.Key == hashedKey, top: 1, cancellationToken: cancellationToken))
        {
            using var doc = JsonDocument.Parse(record.SerializedState);
            return doc.RootElement.Clone();
        }

        return null;
    }
}

public class ThreadStateSchema
{
    // This will be the primary key (e.g. username)
    [VectorStoreKey]
    public required long Key { get; set; }

    // Your serialized AgentThread state as JSON text
    [VectorStoreData]
    public string SerializedState { get; set; } = default!;
}