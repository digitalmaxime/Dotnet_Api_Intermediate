using static System.Int32;
using CarStateMachine.Persistence;

namespace CarStateMachine;


public class CarStateMachine : VehicleStateMachineBase
{
    public sealed override string Name { get; set; }
    public sealed override State CurrentState { get; set; }
    public override void TakeAction(Action action)
    {
        switch (action)
        {
            case Action.Accelerate:
                StateMachine.Accelerate(Min(speed ?? -1, 100));
                return;

            case Action.Decelerate:
                StateMachine.Decelerate(Max(speed ?? -1, 0));
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }

    public sealed override int CurrentSpeed { get; set; }

    public CarStateMachine(CarEntity carEntity, IVehicleStateRepository vehicleStateRepository)
        : base(vehicleStateRepository)
    {
        Name = carEntity.Name;
        CurrentState = carEntity.State;
        CurrentSpeed = carEntity.Speed;
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

        StateMachine?.Configure(State.Flying)
            .Permit(Action.Land, State.Running);

        // string graph = UmlDotGraph.Format(CarState.GetInfo());
    }

    public void Accelerate(int speed)
    {
        StateMachine.Fire(AccelerateWithParam, speed);
    }

    public void Decelerate(int speed)
    {
        StateMachine.Fire(DecelerateWithParam, Max(speed, 0));
    }

    // public void Fly()
    // {
    //     CarState.Fire(Action.Fly);
    // }
    //
    // public void Land()
    // {
    //     CarState.Fire(Action.Land);
    // }
}