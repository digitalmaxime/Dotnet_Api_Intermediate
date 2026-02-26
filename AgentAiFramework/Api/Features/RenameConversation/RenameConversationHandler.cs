using AgentFrameworkChat.Contracts.Repositories;
using MediatR;

namespace AgentFrameworkChat.Features.RenameConversation;

public class RenameConversationHandler(IConversationRepository conversationRepository): IRequestHandler<RenameConversationRequestDto, ValueTask>
{
    public async  Task<ValueTask> Handle(RenameConversationRequestDto request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.GetConversation(request.ConversationId, cancellationToken);
        
        if (conversation == null)
        {
            return ValueTask.FromException(
                new KeyNotFoundException($"Conversation {request.ConversationId} not found for rename operation"));
        }

        await conversationRepository.RenameConversation(request.ConversationId, request.NewConversationTitle,
            cancellationToken);

        return ValueTask.CompletedTask;
    }
}