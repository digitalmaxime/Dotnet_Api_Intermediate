using Microsoft.Extensions.DependencyInjection;
using Stateless;
using StateMachine.Persistence.Contracts;
using StateMachine.Persistence.Domain;
using static System.Int32;

namespace StateMachine.VehicleStateMachines;

public class PlaneStateMachine : IVehicleStateMachine
{
    private readonly IServiceProvider _serviceProvider;

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
    private int CurrentSpeed { get; set; }
    public int Altitude { get; set; }
    private PlaneState CurrentState { get; set; }
    public string GetCurrentState => CurrentState.ToString();
    private readonly StateMachine<PlaneState, PlaneAction> _stateMachine;
    public IEnumerable<string> GetPermittedTriggers => _stateMachine.GetPermittedTriggers().Select(x => x.ToString());


    private StateMachine<PlaneState, PlaneAction>.TriggerWithParameters<int>? _accelerateWithParam;
    private StateMachine<PlaneState, PlaneAction>.TriggerWithParameters<int>? _decelerateWithParam;

    public PlaneStateMachine(string id, IServiceProvider serviceProvider)
    {
        Id = id;
        _serviceProvider = serviceProvider;
        _stateMachine = new StateMachine<PlaneState, PlaneAction>(
            () => CurrentState,
            (s) =>
            {
                CurrentState = s;
                SaveState();
            }
        );
        InitializeStateMachine(id).GetAwaiter();
        ConfigureStates();
    }

    ~PlaneStateMachine()
    {
        Console.WriteLine("~PlaneStateMachine xox");
    }

    private async Task InitializeStateMachine(string id)
    {
        var planeStateRepository = _serviceProvider.GetRequiredService<IPlaneStateRepository>();
        var plane = planeStateRepository.GetById(id);
        if (plane == null)
        {
            plane = new PlaneEntity()
            {
                Id = id, Speed = 0, State = PlaneState.Stopped
            };
            
            await planeStateRepository.Save(plane);
        }

        CurrentState = plane.State;
        CurrentSpeed = plane.Speed;
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
        var planeStateRepository = _serviceProvider.GetRequiredService<IPlaneStateRepository>();
        var plane = new PlaneEntity()
        {
            Id = Id, Speed = CurrentSpeed, State = CurrentState
        };
        
        planeStateRepository.Save(plane);
    }

    public void TakeAction(string actionString)
    {
        Enum.TryParse<PlaneAction>(actionString, out var action);

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
                _stateMachine.Fire(_decelerateWithParam, Max(CurrentSpeed - 35, 0));
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