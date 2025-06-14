namespace Models;

public class WorkTodoEvent : ITodoEvent
{
    public required string Description { get; set; }
}