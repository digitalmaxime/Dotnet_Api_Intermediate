using static System.Int32;

namespace CarStateMachine.CarStateManager;

public class BasicCarStateManager : CarStateManagerBase
{
    protected override void ProcessInputTrigger(CarStateMachine.Action action, int speed, VehicleStateMachineBase carStateMachine)
    {
        switch (action)
        {
            case VehicleStateMachineBase.Action.Stop:
                carStateMachine.Stop();
                break;

            case VehicleStateMachineBase.Action.Start:
                carStateMachine.Start();
                break;

            case VehicleStateMachineBase.Action.Accelerate:
                carStateMachine.Accelerate(Min(speed, 100));
                break;

            case VehicleStateMachineBase.Action.Decelerate:
                carStateMachine.Decelerate(Max(speed, 0));
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}