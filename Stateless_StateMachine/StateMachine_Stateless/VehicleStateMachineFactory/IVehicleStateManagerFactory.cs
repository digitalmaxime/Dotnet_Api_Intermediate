using StateMachine.Persistence.Constants;
using StateMachine.VehicleStateMachines;

namespace StateMachine.VehicleStateMachineFactory;

public interface IVehicleFactory
{
    IVehicleStateMachine CreateVehicleStateMachine(VehicleType type, string vehicleId);
}