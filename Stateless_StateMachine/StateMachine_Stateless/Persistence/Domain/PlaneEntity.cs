using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence.Domain;

public class PlaneEntity: EntityWithId
{
    public int Speed { get; set; }
    public PlaneStateMachine.PlaneState State { get; set; }
}