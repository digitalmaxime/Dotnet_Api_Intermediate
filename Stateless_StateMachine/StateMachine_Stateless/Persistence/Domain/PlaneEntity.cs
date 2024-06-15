using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence.Domain;

public class PlaneEntity: EntityWithId
{
    public string Id { get; set; } = default!; // PK
    public int Speed { get; set; }
    public PlaneStateMachine.PlaneState State { get; set; }
}