using StateMachine.Persistence;
using StateMachine.VehicleStateMachines;

namespace StateMachine.VehicleStateMachineFactory;

public enum VehicleType
{
    Car,
    Plane
}
public interface IVehicleFactory
{
    IVehicleStateMachineBase CreateVehicleStateMachine(VehicleType type, VehicleEntity vehicleEntity, IVehicleStateRepository vehicleStateRepository);
}