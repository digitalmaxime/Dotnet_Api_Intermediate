using MediatR;

namespace AgentFrameworkChat.Features.PinConversation;

public class PinConversationRequestDto: IRequest<ValueTask>
{
    public required Guid ConversationId { get; init; }
    public required bool IsPinned { get; init; }
}