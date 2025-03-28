namespace KafkaFlowDemo.Endpoints;

public record Todo
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public DateOnly? DueDate { get; init; }
}