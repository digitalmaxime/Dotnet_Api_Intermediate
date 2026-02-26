using MediatR;

namespace AgentFrameworkChat.Features.GetConversationMessages;

public class GetConversationMessagesRequestDto: IRequest<GetConversationMessagesResponseDto>
{
    public required Guid ConversationId { get; init; }
}