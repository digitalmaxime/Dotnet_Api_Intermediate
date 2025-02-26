namespace StateMachine.Persistence.Domain;

public abstract class EntityWithId // TODO: Try it out with "interface" instead of "abstract class"
{
    public string Id { get; init; } = null!;
}