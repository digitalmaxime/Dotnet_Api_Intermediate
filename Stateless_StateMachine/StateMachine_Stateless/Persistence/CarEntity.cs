namespace CarStateMachine.Persistence;

public class CarEntity
{
    public string Name { get; set; } = default!; // PK
    public int Speed { get; set; }
    public State State { get; set; }
}