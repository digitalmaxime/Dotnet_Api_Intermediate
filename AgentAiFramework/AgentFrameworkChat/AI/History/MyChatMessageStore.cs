using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.PgVector;

namespace AgentFrameworkChat.AI.History;

[Experimental("MEAI001")]
public class MyChatMessageStore(JsonElement serializedStoreState, string connectionString, string username)
    : ChatMessageStore
{
    private readonly VectorStore _vectorStore = new PostgresVectorStore(connectionString!);
    private string? ThreadDbKey { get; set; } = serializedStoreState.ValueKind is JsonValueKind.String
        ? serializedStoreState.Deserialize<string>()
        : username;

    public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(CancellationToken cancellationToken = new())
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

    public async ValueTask AddMessagesAsync(IEnumerable<ChatMessage> messages,
        CancellationToken cancellationToken = new())
    {
        // TODO: implement reducer
        ThreadDbKey ??= username;
        var firstMessage = messages.First();
        var serializedMessage1 = JsonSerializer.Serialize(firstMessage, AgentAbstractionsJsonUtilities.DefaultOptions);
        
        var collection = _vectorStore.GetCollection<string, ChatHistorySchema>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        await collection.UpsertAsync(messages.Select(x => new ChatHistorySchema
        {
            Key = VectorStoreHelper.GenerateChatMessageIntKey(ThreadDbKey, x.MessageId ?? $"{DateTimeOffset.UtcNow:O}:{Guid.NewGuid():N}"),
            Timestamp = DateTimeOffset.UtcNow,
            ThreadId = ThreadDbKey,
            SerializedMessage = serializedMessage1,
            MessageText = x.Text
        }), cancellationToken);
        
    }

    // We have to serialize the thread id so that on deserialization you can retrieve the messages using the same thread id.
    public override async ValueTask<IEnumerable<ChatMessage>> InvokingAsync(InvokingContext context, CancellationToken cancellationToken = new CancellationToken())
    {
// Ensure we have a stable key for this user/thread.
        ThreadDbKey ??= username;

        // Provide stored history to the agent/model for this turn.
        return await GetMessagesAsync(cancellationToken);    }

    public override async ValueTask InvokedAsync(InvokedContext context, CancellationToken cancellationToken = new CancellationToken())
    {
// Ensure we have a stable key for this user/thread.
        ThreadDbKey ??= username;

        // Persist both the sent request messages and the generated response messages.
        // (If you see duplicates, you can add a de-dupe strategy here based on MessageId/Text/Timestamp.)
        await AddMessagesAsync(context.RequestMessages, cancellationToken);
        await AddMessagesAsync(context.ResponseMessages, cancellationToken);    }

    public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null) =>
        JsonSerializer.SerializeToElement(ThreadDbKey);
    
}