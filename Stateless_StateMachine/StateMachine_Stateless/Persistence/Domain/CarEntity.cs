using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence.Domain;

public class CarEntity: EntityWithId
{
    public int Speed { get; set; }
    public CarStateMachine.CarState State { get; set; }
}