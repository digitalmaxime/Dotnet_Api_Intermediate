using static System.Int32;

namespace CarStateMachine.CarStateManager;

public class FlyingCarStateManager: ICarStateManager
{
    public void ProcessUserInput(string? input)
    {
        var success = Enum.TryParse<Action>(input, out var action);

        if (!success && input != "q")
        {
            Console.WriteLine("\tInvalid input, doesn't match any action");
            return;
        }

        var speed = 1;
        if (input == "Accelerate")
        {
            Console.Write($"Enter speed greater than current speed {Car.CurrentSpeed}");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputtedSpeed);
            if (!parseSuccessful || inputtedSpeed < Car.CurrentSpeed)
                throw new ArgumentException("Invalid argument for Speed");
            speed = inputtedSpeed;
        }

        if (input == "Decelerate")
        {
            Console.Write($"Enter speed < {Car.CurrentSpeed} : ");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputedSpeed);
            if (!parseSuccessful || inputedSpeed > Car.CurrentSpeed)
                throw new ArgumentException("Invalid argument for Speed");
            speed = inputedSpeed;
        }

        switch (action)
        {
            case Action.Stop:
                Car.CarState.Fire(Action.Stop);
                break;

            case Action.Start:
                Car.CarState.Fire(Action.Start);
                break;

            case Action.Accelerate:
                Car.CarState.Fire(Car.AccelerateWithParam, speed);
                break;

            case Action.Decelerate:
                Car.CarState.Fire(Car.DecelerateWithParam, Max(speed, 0));
                break;

            case Action.Fly:
                Car.CarState.Fire(Action.Fly);
                break;

            case Action.Land:
                Car.CarState.Fire(Action.Land);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}