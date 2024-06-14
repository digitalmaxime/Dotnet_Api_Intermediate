using static System.Int32;

namespace CarStateMachine.CarStateManager;

public abstract class CarStateManagerBase
{
    public void ProcessUserInput(string? input, VehicleStateMachineBase carStateMachine)
    {
        var success = Enum.TryParse<VehicleStateMachineBase.Action>(input, out var action);

        if (!success && input != "q")
        {
            Console.WriteLine("\tInvalid input, doesn't match any action");
            return;
        }

        var speed = 1;
        if (input == "Accelerate")
        {
            Console.Write($"Enter speed greater than current speed {carStateMachine.CurrentSpeed} : ");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputtedSpeed);
            if (!parseSuccessful || inputtedSpeed < carStateMachine.CurrentSpeed)
            {
                Console.WriteLine("\tInvalid argument for Speed");
                return;
            }

            speed = inputtedSpeed;
        }

        if (input == "Decelerate")
        {
            Console.Write($"Enter speed < {carStateMachine.CurrentSpeed} : ");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputedSpeed);
            if (!parseSuccessful || inputedSpeed > carStateMachine.CurrentSpeed)
            {
                Console.WriteLine("\tInvalid argument for Speed");
                return;
            }

            speed = inputedSpeed;
        }

        ProcessInputTrigger(action, speed, carStateMachine);
    }

    protected abstract void ProcessInputTrigger(VehicleStateMachineBase.Action action, int speed, VehicleStateMachineBase carStateMachine);
}