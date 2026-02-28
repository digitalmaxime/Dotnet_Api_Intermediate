using MediatR;

namespace Application.Features.GetConversationMessages;

public class GetConversationMessagesRequestDto: IRequest<GetConversationMessagesResponseDto>
{
    public required Guid ConversationId { get; init; }
}