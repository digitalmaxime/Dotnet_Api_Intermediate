using AgentFrameworkChat.Contracts.Repositories;
using MediatR;

namespace AgentFrameworkChat.Features.GetConversationMessages;

public class GetConversationMessagesHandler(IMessageRepository messageRepository): IRequestHandler<GetConversationMessagesRequestDto, GetConversationMessagesResponseDto>
{
    public async Task<GetConversationMessagesResponseDto> Handle(GetConversationMessagesRequestDto request, CancellationToken cancellationToken)
    {
        const int maxConversationSize = 100;
        var messages =
            await messageRepository.GetFilteredChatMessagesAsync(request.ConversationId, maxConversationSize, cancellationToken);

        return new GetConversationMessagesResponseDto()
        {
            Messages = messages
        };
    }
}