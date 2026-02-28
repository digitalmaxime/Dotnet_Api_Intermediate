using MediatR;

namespace Application.Features.RenameConversation;

public class RenameConversationRequestDto: IRequest<ValueTask>
{
    public required Guid ConversationId { get; init; }
    public required string NewConversationTitle { get; init; }
}