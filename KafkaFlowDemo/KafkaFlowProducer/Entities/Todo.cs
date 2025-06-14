namespace KafkaFlowProducer.Entities;

public record Todo
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public DateOnly? DueDate { get; init; }
}