namespace AgentFrameworkChat.Features.Chat;

public record ChatCommandResponseDto
{
    public required string Role { get; init; }
    public required string Type { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public bool IsRequestInError { get; set; }
}