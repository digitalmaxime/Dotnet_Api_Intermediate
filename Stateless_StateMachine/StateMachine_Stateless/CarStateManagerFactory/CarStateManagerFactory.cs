using Car_StateMachine.CarStateManager;
using CarStateMachine.CarStateManager;

namespace CarStateMachine.CarStateManagerFactory;

public class CarStateManagerFactory: ICarStateManagerFactory
{
    public ICarStateManager GetCarStateManager(string type)
    {
        return type switch
        {
            "Basic" => new CarStateManagerBasic(),
            "Premium" => new CarStateManagerPremium(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}