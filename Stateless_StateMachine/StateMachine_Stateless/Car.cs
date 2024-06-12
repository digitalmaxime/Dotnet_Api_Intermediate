using static System.Int32;
using CarStateMachine.Persistence;
using Stateless;
using Stateless.Graph;

namespace CarStateMachine;

public class Car
{
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

    public string Name { get; set; }
    public State CurrentState => _carState.State;
    public int CurrentSpeed { get; private set; }

    private readonly StateMachine<State, Action> _carState;
    public IEnumerable<string> PermittedTriggers => _carState.GetPermittedTriggers().Select(x => x.ToString());

    private StateMachine<State, Action>.TriggerWithParameters<int>? _accelerateWithParam;
    private StateMachine<State, Action>.TriggerWithParameters<int>? _decelerateWithParam;

    public Car(string name, ICarStateRepository carStateRepository)
    {
        var carStateRepository1 = carStateRepository;
        Name = name;
        _carState = new StateMachine<State, Action>(
                () =>
                    carStateRepository1.Get(Name).State,
                (state) =>
                    carStateRepository1?.Save(Name, state, CurrentSpeed)
            );

        ConfigureCarStates();
    }

    public static void PrintState(StateMachine<State, Action>.Transition state)
    {
        Console.WriteLine(
            $"\tOnExit/OnExit\n\tState Source : {state.Source}, " +
            $"State Trigger : {state.Trigger}, " +
            $"State destination : {state.Destination}");
    }

    private void ConfigureCarStates()
    {
        _accelerateWithParam = _carState.SetTriggerParameters<int>(Action.Accelerate);
        _decelerateWithParam = _carState.SetTriggerParameters<int>(Action.Decelerate);

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
            .OnEntryFrom(_accelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            .PermitIf(Action.Stop, State.Stopped, () => CurrentSpeed == 0)
            .PermitIf(Action.Fly, State.Flying, () => CurrentSpeed > 100)
            .InternalTransition<int>(_accelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            .InternalTransitionIf<int>(_decelerateWithParam, (speed) => CurrentSpeed > 0, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            ;

        _carState?.Configure(State.Flying)
            .Permit(Action.Land, State.Running);

        // string graph = UmlDotGraph.Format(_carState.GetInfo());
    }

    public void Stop()
    {
        _carState.Fire(Action.Stop);
    }

    public void Start()
    {
        _carState.Fire(Action.Start);
    }

    public void Accelerate(int speed)
    {
        _carState.Fire(_accelerateWithParam, speed);
    }

    public void Decelerate(int speed)
    {
        _carState.Fire(_decelerateWithParam, Max(speed, 0));
    }

    public void Fly()
    {
        _carState.Fire(Car.Action.Fly);
    }

    public void Land()
    {
        _carState.Fire(Car.Action.Land);
    }
}