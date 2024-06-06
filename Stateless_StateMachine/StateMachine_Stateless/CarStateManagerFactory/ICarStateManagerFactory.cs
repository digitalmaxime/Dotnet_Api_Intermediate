using CarStateMachine.CarStateManager;

namespace CarStateMachine.CarStateManagerFactory;

public interface ICarStateManagerFactory
{
    ICarStateManager GetCarStateManager(string type);
}