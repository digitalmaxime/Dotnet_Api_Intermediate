using StateMachine.Persistence;
using static System.Int32;

namespace StateMachine.VehicleStateMachines;

public class PlaneStateMachine : VehicleStateMachineBase
{
    public sealed override string Name { get; set; }
    public sealed override State CurrentState { get; set; }
    public override void TakeAction(Action action)
    {
        switch (action)
        {
            case Action.Accelerate:
                StateMachine.Fire(AccelerateWithParam, CurrentSpeed + 35);
                return;

            case Action.Decelerate:
                StateMachine.Fire(DecelerateWithParam, Max(CurrentSpeed - 35, 0));
                return;
            
            case Action.Fly:
                StateMachine.Fire(Action.Fly);
                return;

            case Action.Land:
                StateMachine.Fire(Action.Land);
                return;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, $"{nameof(PlaneStateMachine)} does not support {action}");
        }
    }

    public sealed override int CurrentSpeed { get; set; }

    public PlaneStateMachine(VehicleEntity vehicleEntity, IVehicleStateRepository vehicleStateRepository)
        : base(vehicleStateRepository)
    {
        Name = vehicleEntity.Name;
        CurrentState = vehicleEntity.State;
        CurrentSpeed = vehicleEntity.Speed;
    }

    ~PlaneStateMachine()
    {
        Console.WriteLine("~PlaneStateMachine xox");
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
            .PermitIf(Action.Fly, State.Flying, () => CurrentSpeed > 100)
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

        StateMachine.Configure(State.Flying)
            .Permit(Action.Land, State.Running);
    }
}