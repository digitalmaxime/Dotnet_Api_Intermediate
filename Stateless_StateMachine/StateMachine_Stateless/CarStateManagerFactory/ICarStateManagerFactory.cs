using CarStateMachine.CarStateManager;

namespace CarStateMachine.CarStateManagerFactory;

public enum CarType
{
    Basic,
    Flying
}
public interface ICarStateManagerFactory
{
    ICarStateManager GetCarStateManager(CarType type);
}