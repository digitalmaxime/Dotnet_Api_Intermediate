using AgentFrameworkChat.Models;

namespace AgentFrameworkChat.Features.GetConversationMessages;

public record GetConversationMessagesResponseDto
{
    public required IReadOnlyList<MessageModel> Messages { get; init; }
}