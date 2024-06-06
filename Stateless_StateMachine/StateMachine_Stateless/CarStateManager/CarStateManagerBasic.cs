using CarStateMachine.CarStateManager;
using static System.Int32;

namespace Car_StateMachine.CarStateManager;

public class CarStateManagerBasic : ICarStateManager
{
    public void ProcessUserInput(string? input)
    {
        var success = Enum.TryParse<Car_States_Actions.Action>(input, out var action);

        if (!success && input != "q")
        {
            Console.WriteLine("\tInvalid input, doesn't match any action");
            return;
        }

        var speed = 1;
        if (input == "Accelerate")
        {
            Console.WriteLine($"Enter speed geater than current speed {Car.CurrentSpeed}");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputtedSpeed);
            if (!parseSuccessful || inputtedSpeed < Car.CurrentSpeed)
                throw new ArgumentException("Invalid argument for Speed");
            speed = inputtedSpeed;
        }

        if (input == "Decelerate")
        {
            Console.WriteLine($"Enter speed < {Car.CurrentSpeed} : ");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputedSpeed);
            if (!parseSuccessful || inputedSpeed > Car.CurrentSpeed)
                throw new ArgumentException("Invalid argument for Speed");
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
                Car.CarState.Fire(Car.AccelerateWithParam, Min(speed, 100));
                break;

            case Car_States_Actions.Action.Decelerate:
                Car.CarState.Fire(Car.DecelerateWithParam, Max(speed, 1));
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}