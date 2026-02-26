using AgentFrameworkChat.Enums;
using MediatR;

namespace AgentFrameworkChat.Features.ApprovePizzaOrder;

public record ApprovePizzaOrderRequestDto: IRequest<ApprovePizzaOrderResponseDto>
{
    public required string Username { get; init; }
    public required Guid ConversationId { get; init; }
    public required bool IsApproved { get; init; }
    public required Language Language { get; init; }
}