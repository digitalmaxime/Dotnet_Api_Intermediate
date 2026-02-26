using AgentFrameworkChat.Enums;

namespace Infrastructure.Entities;

public class Message
{
    public required Guid MessageId { get; init; }

    public required Guid ConversationId { get; set; }

    public DateTimeOffset Timestamp { get; init; }

    public required string SerializedMessage { get; init; }

    public required MessageRole Role { get; init; }

    public required string MessageText { get; set; }

    public required MessageType Type { get; set; }

    public Conversation Conversation { get; set; } = null!;
}