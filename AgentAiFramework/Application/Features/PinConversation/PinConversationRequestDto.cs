using MediatR;

namespace Application.Features.PinConversation;

public class PinConversationRequestDto: IRequest<ValueTask>
{
    public required Guid ConversationId { get; init; }
    public required bool IsPinned { get; init; }
}