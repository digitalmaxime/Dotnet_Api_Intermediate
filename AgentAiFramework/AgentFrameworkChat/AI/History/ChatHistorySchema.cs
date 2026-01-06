using Microsoft.Extensions.VectorData;

namespace AgentFrameworkChat.AI.History;

public class ChatHistorySchema
{
    
    [VectorStoreKey] public int Key { get; set; }
    [VectorStoreData] public string? ThreadId { get; set; }
    [VectorStoreData] public DateTimeOffset? Timestamp { get; set; }
    [VectorStoreData] public string? SerializedMessage { get; set; }
    [VectorStoreData] public string? MessageText { get; set; }
}