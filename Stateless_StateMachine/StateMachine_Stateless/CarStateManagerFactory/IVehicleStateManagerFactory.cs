using CarStateMachine.CarStateManager;

namespace CarStateMachine.CarStateManagerFactory;

public enum VehicleType
{
    Basic,
    Flying
}
public interface IVehicleStateManagerFactory
{
    IVehicleStateManager GetCarStateManager(VehicleType type);
}