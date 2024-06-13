using static System.Int32;

namespace CarStateMachine.CarStateManager;

public class FlyingCarStateManager: CarStateManagerBase
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
                car.Accelerate(speed);
                break;

            case Car.Action.Decelerate:
                car.Decelerate(Max(speed, 0));
                break;

            case Car.Action.Fly:
                car.Fly();
                break;

            case Car.Action.Land:
                car.Land();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}