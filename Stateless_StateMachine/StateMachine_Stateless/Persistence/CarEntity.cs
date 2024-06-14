using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence;

public class CarEntity
{
    public string Id { get; set; } = default!; // PK
    public int Speed { get; set; }
    public CarStateMachine.CarState State { get; set; }
}