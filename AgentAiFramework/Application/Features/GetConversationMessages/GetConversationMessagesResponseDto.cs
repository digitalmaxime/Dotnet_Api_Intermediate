using Application.Models;

namespace Application.Features.GetConversationMessages;

public record GetConversationMessagesResponseDto
{
    public required IReadOnlyList<MessageModel> Messages { get; init; }
}