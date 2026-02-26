namespace AgentFrameworkChat.Features.ApprovePizzaOrder;

public record ApprovePizzaOrderResponseDto
{
    public required string Role { get; init; }
    public required string Type  { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
}