using static System.Int32;
using CarStateMachine.Persistence;
using Stateless;

namespace CarStateMachine;

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
    public int CurrentSpeed { get; set; }
    public string Name { get; set; }

    public State CurrentState { get; set; }
}

public abstract class VehicleStateMachineBase : IVehicleStateMachineBase
{
    public abstract string Name { get; set; }
    public abstract int CurrentSpeed { get; set; }
    public abstract State CurrentState { get; set; }

    protected readonly StateMachine<State, Action> CarState;
    
    protected readonly IVehicleStateRepository VehicleStateRepository;
    
    public IEnumerable<string> PermittedTriggers => CarState.GetPermittedTriggers().Select(x => x.ToString());
    
    protected StateMachine<State, Action>.TriggerWithParameters<int>? _accelerateWithParam;
    protected StateMachine<State, Action>.TriggerWithParameters<int>? _decelerateWithParam;

    protected VehicleStateMachineBase(IVehicleStateRepository vehicleStateRepository)
    {
        VehicleStateRepository = vehicleStateRepository;

        CarState = new StateMachine<State, Action>(
            () =>
                CurrentState,
            (s) =>
            {
                CurrentState = s;
                SaveState();
            }
        );

        ConfigureCarStates();
    }

    protected void SaveState()
    {
        VehicleStateRepository?.Save(Name, CurrentState, CurrentSpeed);
    }

    public static void PrintState(StateMachine<State, Action>.Transition state)
    {
        Console.WriteLine(
            $"\tOnEntry/OnExit\n\tState Source : {state.Source}, " +
            $"State Trigger : {state.Trigger}, " +
            $"State destination : {state.Destination}");
    }

    protected abstract void ConfigureCarStates();
    
    public void Stop()
    {
        CarState.Fire(Action.Stop);
    }

    public void Start()
    {
        CarState.Fire(Action.Start);
    }

    public void Accelerate(int speed)
    {
        CarState.Fire(_accelerateWithParam, speed);
    }

    public void Decelerate(int speed)
    {
        CarState.Fire(_decelerateWithParam, Max(speed, 0));
    }

}