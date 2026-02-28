using Application.Contracts.Repositories;
using MediatR;

namespace Application.Features.GetUserConversations;

public class GetUserConversationsHandler(IConversationRepository conversationRepository): IRequestHandler<GetUserConversationsRequestDto, GetUserConversationsResponseDto>
{
    public async Task<GetUserConversationsResponseDto> Handle(GetUserConversationsRequestDto request, CancellationToken cancellationToken)
    {
        var conversations = await conversationRepository.GetUserConversations(request.Username, cancellationToken);
        
        return new GetUserConversationsResponseDto()
        {
            Conversations = conversations
        };
    
    }
}