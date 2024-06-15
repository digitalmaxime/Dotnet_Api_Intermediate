using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence;

public class PlaneEntity
{
    public string Id { get; set; } = default!; // PK
    public int Speed { get; set; }
    public PlaneStateMachine.PlaneState State { get; set; }
}