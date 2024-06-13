using static System.Int32;

namespace CarStateMachine.CarStateManager;

public class BasicCarStateManager : CarStateManagerBase
{
    protected override void ProcessInputTrigger(Car.Action action, int speed, Car car)
    {
        switch (action)
        {
            case Car.Action.Stop:
                car.Stop();
                break;

            case Car.Action.Start:
                car.Start();
                break;

            case Car.Action.Accelerate:
                car.Accelerate(Min(speed, 100));
                break;

            case Car.Action.Decelerate:
                car.Decelerate(Max(speed, 0));
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}