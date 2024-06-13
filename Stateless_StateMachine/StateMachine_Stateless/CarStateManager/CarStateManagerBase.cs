using static System.Int32;

namespace CarStateMachine.CarStateManager;

public abstract class CarStateManagerBase
{
    public void ProcessUserInput(string? input, Car car)
    {
        var success = Enum.TryParse<Car.Action>(input, out var action);

        if (!success && input != "q")
        {
            Console.WriteLine("\tInvalid input, doesn't match any action");
            return;
        }

        var speed = 1;
        if (input == "Accelerate")
        {
            Console.Write($"Enter speed greater than current speed {car.CurrentSpeed} : ");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputtedSpeed);
            if (!parseSuccessful || inputtedSpeed < car.CurrentSpeed)
            {
                Console.WriteLine("\tInvalid argument for Speed");
                return;
            }

            speed = inputtedSpeed;
        }

        if (input == "Decelerate")
        {
            Console.Write($"Enter speed < {car.CurrentSpeed} : ");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputedSpeed);
            if (!parseSuccessful || inputedSpeed > car.CurrentSpeed)
            {
                Console.WriteLine("\tInvalid argument for Speed");
                return;
            }

            speed = inputedSpeed;
        }

        ProcessInputTrigger(action, speed, car);
    }

    protected abstract void ProcessInputTrigger(Car.Action action, int speed, Car car);
}