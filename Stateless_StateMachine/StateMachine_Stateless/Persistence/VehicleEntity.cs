using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence;

public class VehicleEntity
{
    public string Name { get; set; } = default!; // PK
    public int Speed { get; set; }
    public State State { get; set; }
}