using CarStateMachine.Persistence;
using Stateless;

namespace CarStateMachine;

public class CarStateMachine
{
    public string Name { get; set; }
    
    private readonly ICarStateRepository _carStateRepository;

    private readonly StateMachine<State, Action> _carState;
    
    public int CurrentSpeed { get; private set; }
    
    public State CurrentState { get; private set; }

    public CarStateMachine(ICarStateRepository carStateRepository)
    {
        _carStateRepository = carStateRepository;
        _carState = new StateMachine<State, Action>(
            () =>
            {
                return _carStateRepository.Get("default").CurrentState;
            },
            (state) =>
            {
                _carStateRepository?.Save(this);
            });


        ConfigureCarStates();
    }

    private static void PrintState(StateMachine<State, Action>.Transition state)
    {
        Console.WriteLine(
            $"\tOnExit/OnExit\n\tState Source : {state.Source}, " +
            $"State Trigger : {state.Trigger}, " +
            $"State destination : {state.Destination}");
    }

    private void ConfigureCarStates()
    {
        StateMachine<State, Action>.TriggerWithParameters<int>? accelerateWithParam = _carState.SetTriggerParameters<int>(Action.Accelerate);
        StateMachine<State, Action>.TriggerWithParameters<int>? decelerateWithParam = _carState.SetTriggerParameters<int>(Action.Decelerate);

        _carState.Configure(State.Stopped)
            .Permit(Action.Start, State.Started)
            // .Ignore(Action.Stop) // --> same as .PermitReentry(Action.Stop)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState)
            ;

        _carState.Configure(State.Started)
            .Permit(Action.Accelerate, State.Running)
            .Permit(Action.Stop, State.Stopped)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState)
            ;

        _carState.Configure(State.Running)
            .OnEntryFrom(accelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            .PermitIf(Action.Stop, State.Stopped, () => CurrentSpeed == 0)
            .PermitIf(Action.Fly, State.Flying, () => CurrentSpeed > 100)
            .InternalTransition<int>(accelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            .InternalTransitionIf<int>(decelerateWithParam, (speed) => CurrentSpeed > 0, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            ;

        _carState?.Configure(State.Flying)
            .Permit(Action.Land, State.Running);
    }
}