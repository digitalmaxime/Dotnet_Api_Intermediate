using StateMachine.VehicleStateMachines;

namespace StateMachine.VehicleStateMachineFactory;

public enum VehicleType
{
    Car,
    Plane
}
public interface IVehicleFactory
{
    IVehicleStateMachine CreateVehicleStateMachine(VehicleType type, string vehicleId);
}