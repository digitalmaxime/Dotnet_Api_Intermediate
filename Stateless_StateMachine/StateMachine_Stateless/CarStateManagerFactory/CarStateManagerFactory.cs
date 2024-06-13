using CarStateMachine.CarStateManager;

namespace CarStateMachine.CarStateManagerFactory;

public class CarStateManagerFactory: ICarStateManagerFactory
{
    public CarStateManager.CarStateManagerBase GetCarStateManager(CarType type)
    {
        return type switch
        {
            CarType.Basic => new BasicCarStateManager(),
            CarType.Flying => new FlyingCarStateManager(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}