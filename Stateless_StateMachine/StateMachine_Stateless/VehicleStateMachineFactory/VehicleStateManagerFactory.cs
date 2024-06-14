using StateMachine.Persistence;
using StateMachine.VehicleStateMachines;

namespace StateMachine.VehicleStateMachineFactory;

public class VehicleFactory: IVehicleFactory
{
    public IVehicleStateMachineBase CreateVehicleStateMachine(VehicleType type, VehicleEntity vehicleEntity, IVehicleStateRepository vehicleStateRepository)
    {
        return type switch
        {
            VehicleType.Car => new CarStateMachine(vehicleEntity, vehicleStateRepository),
            VehicleType.Plane => new PlaneStateMachine(vehicleEntity, vehicleStateRepository),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}