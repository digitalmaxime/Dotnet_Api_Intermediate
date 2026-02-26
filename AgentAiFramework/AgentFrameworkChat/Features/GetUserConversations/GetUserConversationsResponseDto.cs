using AgentFrameworkChat.Models;

namespace AgentFrameworkChat.Features.GetUserConversations;

public record GetUserConversationsResponseDto
{
    public required ICollection<ConversationModel> Conversations { get; init; }
}