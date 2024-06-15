using Stateless;
using Stateless.Graph;
using StateMachine.Persistence;
using static System.Int32;

namespace StateMachine.VehicleStateMachines;

public class CarStateMachine : IVehicleStateMachine
{
    public enum CarState
    {
        Stopped,
        Started,
        Running,
        Drifting,
    }

    public enum CarAction
    {
        Stop,
        Start,
        Accelerate,
        Decelerate,
        Drift
    }

    public string Id { get; set; }
    private int CurrentSpeed { get; set; }
    private CarState CurrentCarState { get; set; }
    public string GetCurrentState => CurrentCarState.ToString();
    public IEnumerable<string> GetPermittedTriggers => _stateMachine.GetPermittedTriggers().Select(x => x.ToString());

    private StateMachine<CarState, CarAction> _stateMachine;
    private readonly ICarStateRepository _carStateRepository;

    private StateMachine<CarState, CarAction>.TriggerWithParameters<int>? _accelerateWithParam;
    private StateMachine<CarState, CarAction>.TriggerWithParameters<int>? _decelerateWithParam;


    public CarStateMachine(string id, ICarStateRepository carStateRepository)
    {
        Id = id;
        _carStateRepository = carStateRepository;
        InitializeStateMachine(id);
        ConfigureStates();
    }

    private void InitializeStateMachine(string id)
    {
        var car = _carStateRepository.GetById(id);
        if (car == null)
        {
            _carStateRepository.Save(id, CarState.Stopped, speed: 0);
        }

        CurrentCarState = car?.State ?? CarState.Stopped;
        CurrentSpeed = car?.Speed ?? -1;

        _stateMachine = new StateMachine<CarState, CarAction>(
            () => CurrentCarState,
            (s) =>
            {
                CurrentCarState = s;
                SaveState();
            }
        );
    }

    ~CarStateMachine()
    {
        Console.WriteLine("~CarStateMachine xox");
    }

    private void ConfigureStates()
    {
        _accelerateWithParam = _stateMachine.SetTriggerParameters<int>(CarAction.Accelerate);
        _decelerateWithParam = _stateMachine.SetTriggerParameters<int>(CarAction.Decelerate);

        _stateMachine.Configure(CarState.Stopped)
            .Permit(CarAction.Start, CarState.Started)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState);

        _stateMachine.Configure(CarState.Started)
            .Permit(CarAction.Accelerate, CarState.Running)
            .Permit(CarAction.Stop, CarState.Stopped)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState);
        _stateMachine.Configure(CarState.Running)
            .OnEntryFrom(_accelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                SaveState();
                Console.WriteLine($"\tSpeed is {CurrentSpeed}");
            })
            .PermitIf(CarAction.Stop, CarState.Stopped, () => CurrentSpeed == 0)
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
    }

    private void SaveState()
    {
        _carStateRepository?.Save(Id, CurrentCarState, CurrentSpeed);
    }

    public void TakeAction(string carActionStr)
    {
        Enum.TryParse<CarAction>(carActionStr, out var carAction);

        switch (carAction)
        {
            case CarAction.Stop:
                _stateMachine.Fire(CarAction.Stop);
                return;

            case CarAction.Start:
                _stateMachine.Fire(CarAction.Start);
                return;

            case CarAction.Accelerate:
                _stateMachine.Fire(_accelerateWithParam, Min(CurrentSpeed + 25, 100));
                return;

            case CarAction.Decelerate:
                _stateMachine.Fire(_decelerateWithParam, Max(CurrentSpeed - 25, 0));
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(carAction), carAction,
                    $"{nameof(CarStateMachine)} does not support {carAction}");
        }
    }

    private static void PrintState(StateMachine<CarState, CarAction>.Transition state)
    {
        Console.WriteLine(
            $"\tOnEntry/OnExit\n\tState Source : {state.Source}, " +
            $"State Trigger : {state.Trigger}, " +
            $"State destination : {state.Destination}");
    }

    public string GetGraph()
    {
        return UmlDotGraph.Format(_stateMachine.GetInfo());
    }
}