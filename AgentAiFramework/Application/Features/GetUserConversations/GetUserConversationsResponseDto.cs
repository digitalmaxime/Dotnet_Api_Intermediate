using Application.Models;

namespace Application.Features.GetUserConversations;

public record GetUserConversationsResponseDto
{
    public required ICollection<ConversationModel> Conversations { get; init; }
}