using static System.Int32;

namespace CarStateMachine.CarStateManager;

public class CarStateManager : IVehicleStateManager
{
    public void ProcessInputTrigger(Action action, IVehicleStateMachineBase carStateMachine)
    {
        int? speed = null;
        if (action == Action.Accelerate)
        {
            Console.Write($"Enter speed greater than current speed {carStateMachine.CurrentSpeed} : ");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputtedSpeed);
            if (!parseSuccessful || inputtedSpeed < carStateMachine.CurrentSpeed)
            {
                Console.WriteLine("\tInvalid argument for Speed");
                throw new ArgumentException("Invalid argument for Speed");
            }

            speed = inputtedSpeed;
        }

        if (action == Action.Decelerate)
        {
            Console.Write($"Enter speed < {carStateMachine.CurrentSpeed} : ");
            var parseSuccessful = TryParse(Console.ReadLine(), out var inputtedSpeed);
            if (!parseSuccessful || inputtedSpeed > carStateMachine.CurrentSpeed)
            {
                Console.WriteLine("\tInvalid argument for Speed");
                throw new ArgumentException("Invalid argument for Speed");
            }

            speed = inputtedSpeed;
        }

        var toto = carStateMachine.PermittedTriggers;

        carStateMachine.TakeAction(action);

    }
}