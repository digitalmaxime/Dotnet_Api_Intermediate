using Stateless;
using StateMachine.Persistence;
using static System.Int32;

namespace StateMachine.VehicleStateMachines;

public class PlaneStateMachine : IVehicleStateMachine
{
    private readonly IPlaneStateRepository _planeStateRepository;

    public enum PlaneState
    {
        Stopped,
        Started,
        Running,
        Flying,
    }

    public enum PlaneAction
    {
        Stop,
        Start,
        Accelerate,
        Decelerate,
        Fly,
        Land
    }

    public string Id { get; set; }
    private PlaneState CurrentPlaneState { get; set; }
    private StateMachine<PlaneState, PlaneAction> _stateMachine;
    public string GetCurrentState => CurrentPlaneState.ToString();
    private int CurrentSpeed { get; set; }
    public int Altitude { get; set; }

    public IEnumerable<string> GetPermittedTriggers => _stateMachine.GetPermittedTriggers().Select(x => x.ToString());

    private StateMachine<PlaneState, PlaneAction>.TriggerWithParameters<int>? _accelerateWithParam;
    private StateMachine<PlaneState, PlaneAction>.TriggerWithParameters<int>? _decelerateWithParam;

    public PlaneStateMachine(string id, IPlaneStateRepository planeStateRepository)
    {
        _planeStateRepository = planeStateRepository;
        Id = id;
        InitializeStateMachine(id);
        ConfigureStates();
    }

    ~PlaneStateMachine()
    {
        Console.WriteLine("~PlaneStateMachine xox");
    }

    private void InitializeStateMachine(string id)
    {
        var plane = _planeStateRepository.GetById(id);
        if (plane == null)
        {
            _planeStateRepository.Save(id, PlaneState.Stopped, speed: 0);
        }

        CurrentPlaneState = plane?.State ?? PlaneState.Stopped;
        CurrentSpeed = plane?.Speed ?? -1;

        _stateMachine = new StateMachine<PlaneState, PlaneAction>(
            () => CurrentPlaneState,
            (s) =>
            {
                CurrentPlaneState = s;
                SaveState();
            }
        );
    }
    
    private void ConfigureStates()
    {
        _accelerateWithParam = _stateMachine.SetTriggerParameters<int>(PlaneAction.Accelerate);
        _decelerateWithParam = _stateMachine.SetTriggerParameters<int>(PlaneAction.Decelerate);

        _stateMachine.Configure(PlaneState.Stopped)
            .Permit(PlaneAction.Start, PlaneState.Started)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState);

        _stateMachine.Configure(PlaneState.Started)
            .Permit(PlaneAction.Accelerate, PlaneState.Running)
            .Permit(PlaneAction.Stop, PlaneState.Stopped)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState);
        _stateMachine.Configure(PlaneState.Running)
            .OnEntryFrom(_accelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                SaveState();
                Console.WriteLine($"\tSpeed is {CurrentSpeed}");
            })
            .PermitIf(PlaneAction.Stop, PlaneState.Stopped, () => CurrentSpeed == 0)
            .PermitIf(PlaneAction.Fly, PlaneState.Flying, () => CurrentSpeed > 100)
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

        _stateMachine.Configure(PlaneState.Flying)
            .Permit(PlaneAction.Land, PlaneState.Running);
    }

    private void SaveState()
    {
        _planeStateRepository?.Save(Id, CurrentPlaneState, CurrentSpeed);
    }

    public void TakeAction(string actionStr)
    {
        Enum.TryParse<PlaneAction>(actionStr, out var action);

        switch (action)
        {
            case PlaneAction.Stop:
                _stateMachine.Fire(PlaneAction.Stop);
                return;

            case PlaneAction.Start:
                _stateMachine.Fire(PlaneAction.Start);
                return;

            case PlaneAction.Accelerate:
                _stateMachine.Fire(_accelerateWithParam, CurrentSpeed + 35);
                return;

            case PlaneAction.Decelerate:
                _stateMachine.Fire(_decelerateWithParam, Max(CurrentSpeed - 45, 0));
                return;
            
            case PlaneAction.Fly:
                _stateMachine.Fire(PlaneAction.Fly);
                return;
            
            case PlaneAction.Land:
                _stateMachine.Fire(PlaneAction.Land);
                return;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action,
                    $"{nameof(PlaneStateMachine)} does not support {action}");
        }
    }

    private static void PrintState(StateMachine<PlaneState, PlaneAction>.Transition state)
    {
        Console.WriteLine(
            $"\tOnEntry/OnExit\n\tState Source : {state.Source}, " +
            $"State Trigger : {state.Trigger}, " +
            $"State destination : {state.Destination}");
    }
}