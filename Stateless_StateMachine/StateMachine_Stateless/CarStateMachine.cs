using CarStateMachine.Persistence;

namespace CarStateMachine;

public interface ICarStateMachine
{
    
    public string Name { get; set; }

    public VehicleStateMachineBase.State CurrentState { get; set; }

    public int CurrentSpeed { get; }
    public IEnumerable<string> PermittedTriggers { get; }
}

public class CarStateMachine : VehicleStateMachineBase, ICarStateMachine
{
    public sealed override string Name { get; set; }
    public sealed override State CurrentState { get; set; }
    public sealed override int CurrentSpeed { get; set; }

    public CarStateMachine(CarEntity carEntity, IVehicleStateRepository vehicleStateRepository)
        : base(vehicleStateRepository)
    {
        Name = carEntity.Name;
        CurrentState = carEntity.State;
        CurrentSpeed = carEntity.Speed;
    }

    protected override void ConfigureCarStates()
    {
        _accelerateWithParam = CarState.SetTriggerParameters<int>(Action.Accelerate);
        _decelerateWithParam = CarState.SetTriggerParameters<int>(Action.Decelerate);

        CarState.Configure(State.Stopped)
            .Permit(Action.Start, State.Started)
            // .Ignore(Action.Stop) // --> same as .PermitReentry(Action.Stop)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState);

        CarState.Configure(State.Started)
            .Permit(Action.Accelerate, State.Running)
            .Permit(Action.Stop, State.Stopped)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState);

        CarState.Configure(State.Running)
            .OnEntryFrom(_accelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                SaveState();
                Console.WriteLine($"\tSpeed is {CurrentSpeed}");
            })
            .PermitIf(Action.Stop, State.Stopped, () => CurrentSpeed == 0)
            .PermitIf(Action.Fly, State.Flying, () => CurrentSpeed > 100)
            .InternalTransition<int>(_accelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                SaveState();
                Console.WriteLine($"\tSpeed is {CurrentSpeed}");
            })
            .InternalTransitionIf<int>(_decelerateWithParam, _ => CurrentSpeed > 0, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {CurrentSpeed}");
            });

        CarState?.Configure(State.Flying)
            .Permit(Action.Land, State.Running);

        // string graph = UmlDotGraph.Format(CarState.GetInfo());
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