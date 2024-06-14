using Stateless;
using StateMachine.Persistence;

namespace StateMachine;

public enum State
{
    Stopped,
    Started,
    Running,
    Flying,
}

public enum Action
{
    Stop,
    Start,
    Accelerate,
    Decelerate,
    Fly,
    Land
}

public interface IVehicleStateMachineBase
{
    public string Name { get; set; }

    public State CurrentState { get; }

    public IEnumerable<Action> PermittedTriggers { get; }

    public void TakeActionBase(Action action);
}

public abstract class VehicleStateMachineBase : IVehicleStateMachineBase
{
    public abstract string Name { get; set; }
    public abstract int CurrentSpeed { get; set; }
    public abstract State CurrentState { get; set; }
    public IEnumerable<Action> PermittedTriggers => StateMachine.GetPermittedTriggers();

    public abstract void TakeAction(Action action);

    public void TakeActionBase(Action action)
    {
        switch (action)
        {
            case Action.Stop:
                StateMachine.Fire(Action.Stop);
                return;

            case Action.Start:
                StateMachine.Fire(Action.Start);
                return;

            default:
                TakeAction(action);
                return;
        }
    }

    protected readonly StateMachine<State, Action> StateMachine;
    private readonly IVehicleStateRepository _vehicleStateRepository;

    protected StateMachine<State, Action>.TriggerWithParameters<int>? AccelerateWithParam;
    protected StateMachine<State, Action>.TriggerWithParameters<int>? DecelerateWithParam;

    protected VehicleStateMachineBase(IVehicleStateRepository vehicleStateRepository)
    {
        _vehicleStateRepository = vehicleStateRepository;

        StateMachine = new StateMachine<State, Action>(
            () =>
                CurrentState,
            (s) =>
            {
                CurrentState = s;
                SaveState();
            }
        );

        ConfigureVehicleStates();
    }

    protected void SaveState()
    {
        _vehicleStateRepository?.Save(Name, CurrentState, CurrentSpeed);
    }

    private static void PrintState(StateMachine<State, Action>.Transition state)
    {
        Console.WriteLine(
            $"\tOnEntry/OnExit\n\tState Source : {state.Source}, " +
            $"State Trigger : {state.Trigger}, " +
            $"State destination : {state.Destination}");
    }

    private void ConfigureVehicleStates()
    {
        StateMachine.Configure(State.Stopped)
            .Permit(Action.Start, State.Started)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState);

        StateMachine.Configure(State.Started)
            .Permit(Action.Accelerate, State.Running)
            .Permit(Action.Stop, State.Stopped)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState);

        ConfigureStates();
    }

    protected abstract void ConfigureStates();
}