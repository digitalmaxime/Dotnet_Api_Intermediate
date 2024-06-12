namespace CarStateMachine.Persistence;

public class CarEntity
{
    public string Name { get; set; } = default!; // PK
    public int Speed { get; set; }
    public Car.State State { get; set; }
}