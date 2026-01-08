using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.PgVector;

namespace AgentFrameworkChat.AI.History;

public class MyChatMessageStore(JsonElement serializedStoreState, string connectionString, string username)
    : ChatMessageStore
{
    private readonly VectorStore _vectorStore = new PostgresVectorStore(connectionString!);
    private string? ThreadDbKey { get; set; } = serializedStoreState.ValueKind is JsonValueKind.String
        ? serializedStoreState.Deserialize<string>()
        : username;

    public override async Task<IEnumerable<ChatMessage>> GetMessagesAsync(CancellationToken cancellationToken = new())
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

    public override async Task AddMessagesAsync(IEnumerable<ChatMessage> messages,
        CancellationToken cancellationToken = new())
    {
        // TODO: implement reducer
        ThreadDbKey ??= username;
        
        var collection = _vectorStore.GetCollection<string, ChatHistorySchema>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        await collection.UpsertAsync(messages.Select(x => new ChatHistorySchema
        {
            Key = VectorStoreHelper.GenerateChatMessageIntKey(ThreadDbKey, x.MessageId),
            Timestamp = DateTimeOffset.UtcNow,
            ThreadId = ThreadDbKey,
            SerializedMessage = JsonSerializer.Serialize(x),
            MessageText = x.Text
        }), cancellationToken);
    }

    // We have to serialize the thread id so that on deserialization you can retrieve the messages using the same thread id.
    public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null) =>
        JsonSerializer.SerializeToElement(ThreadDbKey);
    
}