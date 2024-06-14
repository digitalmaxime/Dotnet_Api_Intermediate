using StateMachine.Persistence;
using static System.Int32;

namespace StateMachine.VehicleStateMachines;

public class CarStateMachine : VehicleStateMachineBase
{
    public sealed override string Name { get; set; }
    public sealed override State CurrentState { get; set; }
    public override void TakeAction(Action action)
    {
        switch (action)
        {
            case Action.Accelerate:
                StateMachine.Fire(AccelerateWithParam, Min(CurrentSpeed + 25, 100));
                return;

            case Action.Decelerate:
                StateMachine.Fire(DecelerateWithParam, Max(CurrentSpeed - 25, 0));
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, $"{nameof(CarStateMachine)} does not support {action}");
        }
    }

    public sealed override int CurrentSpeed { get; set; }

    public CarStateMachine(VehicleEntity vehicleEntity, IVehicleStateRepository vehicleStateRepository)
        : base(vehicleStateRepository)
    {
        Name = vehicleEntity.Name;
        CurrentState = vehicleEntity.State;
        CurrentSpeed = vehicleEntity.Speed;
    }

    ~CarStateMachine()
    {
        Console.WriteLine("~CarStateMachine xox");
    }

    protected override void ConfigureStates()
    {
        AccelerateWithParam = StateMachine.SetTriggerParameters<int>(Action.Accelerate);
        DecelerateWithParam = StateMachine.SetTriggerParameters<int>(Action.Decelerate);

        StateMachine.Configure(State.Running)
            .OnEntryFrom(AccelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                SaveState();
                Console.WriteLine($"\tSpeed is {CurrentSpeed}");
            })
            .PermitIf(Action.Stop, State.Stopped, () => CurrentSpeed == 0)
            .InternalTransition<int>(AccelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                SaveState();
                Console.WriteLine($"\tSpeed is {CurrentSpeed}");
            })
            .InternalTransitionIf<int>(DecelerateWithParam, _ => CurrentSpeed > 0, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {CurrentSpeed}");
            });
    }
}