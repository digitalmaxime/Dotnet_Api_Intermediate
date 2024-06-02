namespace Car_StateMachine.CarStateManagerFactory;

public interface ICarStateManagerFactory
{
    ICarStateManager GetCarStateManager(string type);
}