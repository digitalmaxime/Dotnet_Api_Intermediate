using CarStateMachine.CarStateManager;

namespace CarStateMachine.CarStateManagerFactory;

public class VehicleStateManagerFactory: IVehicleStateManagerFactory
{
    public IVehicleStateManager GetCarStateManager(VehicleType type)
    {
        return type switch
        {
            VehicleType.Basic => new CarStateManager.CarStateManager(),
            // CarType.Flying => new FlyingCarStateManager(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}