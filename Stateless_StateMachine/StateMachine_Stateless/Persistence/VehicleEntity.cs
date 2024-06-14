using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence;

public class VehicleEntity
{
    public string Id { get; set; } = default!; // PK
    public int Speed { get; set; }
}