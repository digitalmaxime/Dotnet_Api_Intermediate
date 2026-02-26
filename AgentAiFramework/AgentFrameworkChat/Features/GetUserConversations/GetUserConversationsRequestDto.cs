using MediatR;

namespace AgentFrameworkChat.Features.GetUserConversations;

public record GetUserConversationsRequestDto: IRequest<GetUserConversationsResponseDto>
{
    public required string Username { get; init; }
}