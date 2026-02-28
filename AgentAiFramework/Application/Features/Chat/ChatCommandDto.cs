using Application.Enums;
using MediatR;

namespace Application.Features.Chat;

public record ChatCommandDto : IRequest<ChatCommandResponseDto>
{
    public required string Username { get; init; }
    public required Guid? ConversationId { get; init; }
    public required string Message { get; init; }
    public required Language Language { get; init; }
    public required string CorrelationId { get; init; }
}