using MediatR;

namespace AgentFrameworkChat.Features.DeleteConversation;

public class DeleteConversationRequestDto: IRequest<ValueTask>
{
    public required Guid ConversationId { get; init; }
    
    public required string Username { get; init; }
}