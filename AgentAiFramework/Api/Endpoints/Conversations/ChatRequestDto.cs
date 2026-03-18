using Application.Enums;

namespace AgentFrameworkChat.Endpoints.Conversations;

public record ChatRequestDto
{
    public required Guid? ConversationId { get; init; }
    public required string Message { get; init; }
    public required string CorrelationId { get; init; }
}