using System.Text.Json;
using AgentFrameworkChat.Options;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.PgVector;

namespace AgentFrameworkChat.AI.History;

public class MyChatMessageStore : ChatMessageStore
{
    private readonly string _username;
    private readonly VectorStore _vectorStore;
    private string? ThreadDbKey { get; set; }

    public MyChatMessageStore(JsonElement serializedStoreState, string connectionString, string username)
    {
        _username = username;
        _vectorStore = new PostgresVectorStore(connectionString!);
        if (serializedStoreState.ValueKind is JsonValueKind.String)
        {
            ThreadDbKey = serializedStoreState.Deserialize<string>();
        }
    }

    public async override Task<IEnumerable<ChatMessage>> GetMessagesAsync(CancellationToken cancellationToken = new())
    {
        var collection = _vectorStore.GetCollection<string, ChatHistorySchema>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        var records = collection
            .GetAsync(
                x => x.ThreadId == ThreadDbKey, top: 10,
                new FilteredRecordRetrievalOptions<ChatHistorySchema>
                    { OrderBy = x => x.Descending(y => y.Timestamp) },
                cancellationToken);
        List<ChatMessage> messages = [];
        await foreach (var record in records)
        {
            messages.Add(JsonSerializer.Deserialize<ChatMessage>(record.SerializedMessage!)!);
        }

        messages.Reverse();
        return messages;
    }

    public async override Task AddMessagesAsync(IEnumerable<ChatMessage> messages,
        CancellationToken cancellationToken = new())
    {
        ThreadDbKey ??= _username;
        var collection = _vectorStore.GetCollection<string, ChatHistorySchema>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        await collection.UpsertAsync(messages.Select(x => new ChatHistorySchema
        {
            Key = GenerateIntKey(ThreadDbKey, x.MessageId),
            Timestamp = DateTimeOffset.UtcNow,
            ThreadId = ThreadDbKey,
            SerializedMessage = JsonSerializer.Serialize(x),
            MessageText = x.Text
        }), cancellationToken);
    }

    // We have to serialize the thread id so that on deserialization you can retrieve the messages using the same thread id.
    public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null) =>
        JsonSerializer.SerializeToElement(ThreadDbKey);
    
    private static int GenerateIntKey(string threadId, string messageId)
    {
        // Create a stable hash from the composite key
        var compositeKey = $"{threadId}_{messageId}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(compositeKey));
        
        // Take first 4 bytes and convert to int
        // Use Math.Abs to ensure positive integer
        return Math.Abs(BitConverter.ToInt32(hash, 0));
    }
}