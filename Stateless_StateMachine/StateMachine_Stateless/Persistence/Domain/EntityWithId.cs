namespace StateMachine.Persistence.Domain;

public abstract class EntityWithId
{
    public string Id { get; init; } = null!;
}