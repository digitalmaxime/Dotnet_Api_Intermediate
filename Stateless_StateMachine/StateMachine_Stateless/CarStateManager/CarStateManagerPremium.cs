using Car_StateMachine;

namespace CarStateMachine.CarStateManager;

public class CarStateManagerPremium: ICarStateManager
{
    public void ProcessUserInput(string? input)
    {
        var success = Enum.TryParse<Car_States_Actions.Action>(input, out var action);

        if (!success && input != "q")
        {
            Console.WriteLine("\tInvalid input, doesn't match any action");
            return;
        }
        
        switch (action)
        {
            case Car_States_Actions.Action.Stop:
                Car.CarState.Fire(Car_States_Actions.Action.Stop);
                break;
            
            case Car_States_Actions.Action.Start:
                Car.CarState.Fire(Car_States_Actions.Action.Start);
                break;
            
            case Car_States_Actions.Action.Accelerate:
                Car.CarState.Fire(Car_States_Actions.Action.Accelerate);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}