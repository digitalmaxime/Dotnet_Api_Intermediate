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
            Console.Write($"Enter speed greater than current speed {CarStateMachine.CurrentSpeed}");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputtedSpeed);
            if (!parseSuccessful || inputtedSpeed < CarStateMachine.CurrentSpeed)
                throw new ArgumentException("Invalid argument for Speed");
            speed = inputtedSpeed;
        }

        if (input == "Decelerate")
        {
            Console.Write($"Enter speed < {CarStateMachine.CurrentSpeed} : ");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputedSpeed);
            if (!parseSuccessful || inputedSpeed > CarStateMachine.CurrentSpeed)
                throw new ArgumentException("Invalid argument for Speed");
            speed = inputedSpeed;
        }

        switch (action)
        {
            case Action.Stop:
                CarStateMachine.CarState.Fire(Action.Stop);
                break;

            case Action.Start:
                CarStateMachine.CarState.Fire(Action.Start);
                break;

            case Action.Accelerate:
                CarStateMachine.CarState.Fire(CarStateMachine.AccelerateWithParam, speed);
                break;

            case Action.Decelerate:
                CarStateMachine.CarState.Fire(CarStateMachine.DecelerateWithParam, Max(speed, 0));
                break;

            case Action.Fly:
                CarStateMachine.CarState.Fire(Action.Fly);
                break;

            case Action.Land:
                CarStateMachine.CarState.Fire(Action.Land);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}