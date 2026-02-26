using AgentFrameworkChat.Contracts.Repositories;
using MediatR;

namespace AgentFrameworkChat.Features.PinConversation;

public class PinConversationHandler(IConversationRepository conversationRepository): IRequestHandler<PinConversationRequestDto, ValueTask>
{
    public async Task<ValueTask> Handle(PinConversationRequestDto request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.GetConversation(request.ConversationId, cancellationToken);

        if (conversation == null)
        {
            return ValueTask.FromException(
                new KeyNotFoundException($"Conversation {request.ConversationId} not found for pin operation"));
        }
        
        await conversationRepository.PinConversation(request.ConversationId, request.IsPinned, cancellationToken);
        
        return ValueTask.CompletedTask;
    }
}