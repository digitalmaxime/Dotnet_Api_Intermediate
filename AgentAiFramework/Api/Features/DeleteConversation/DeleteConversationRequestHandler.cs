using AgentFrameworkChat.Contracts.Repositories;
using MediatR;

namespace AgentFrameworkChat.Features.DeleteConversation;

public class DeleteConversationRequestHandler(IConversationRepository conversationRepository): IRequestHandler<DeleteConversationRequestDto, ValueTask>
{
    public async Task<ValueTask> Handle(DeleteConversationRequestDto request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.GetConversation(request.ConversationId, cancellationToken);

        if (conversation == null)
        {
            return ValueTask.FromException(
                new KeyNotFoundException($"Conversation with id {request.ConversationId} not found"));
        }

        if (!conversation.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException($"Trying to archive conversation of another user");
        }
        
        await conversationRepository.ArchiveUserConversation(request.ConversationId, cancellationToken);
        
        return ValueTask.CompletedTask;
    }
}