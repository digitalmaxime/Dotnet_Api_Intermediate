namespace Models;

public class TrainingTodoEvent : ITodoEvent
{
    public required string Description { get; set; }
}