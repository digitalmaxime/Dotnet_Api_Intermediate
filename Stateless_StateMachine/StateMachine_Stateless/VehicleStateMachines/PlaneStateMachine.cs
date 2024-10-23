using Microsoft.Extensions.DependencyInjection;
using Stateless;
using StateMachine.Application;
using StateMachine.Persistence.Contracts;
using StateMachine.Persistence.Domain;
using static System.Int32;

namespace StateMachine.VehicleStateMachines;

public class PlaneStateMachine : IVehicleStateMachine
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

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
    private readonly StateMachine<PlaneState, PlaneAction> _stateMachine;
    private int CurrentSpeed { get; set; }
    public int Altitude { get; set; }
    private PlaneState CurrentState { get; set; }
    public string GetCurrentState => GetState().ToString();

    public IEnumerable<string> GetPermittedTriggers => _stateMachine
        .GetPermittedTriggers()
        .Select(x => x.ToString());

    private StateMachine<PlaneState, PlaneAction>.TriggerWithParameters<int> _triggerWithSpeedParameter;

    public PlaneStateMachine(string id, IServiceScopeFactory serviceScopeFactory)
    {
        Id = id;
        _serviceScopeFactory = serviceScopeFactory;
        _stateMachine = new StateMachine<PlaneState, PlaneAction>(GetState, SaveState);

        InitializeStateMachine(id).GetAwaiter();
        PlaneStateMachineConfigurator.ConfigureStates(_stateMachine, _triggerWithSpeedParameter);
    }

    ~PlaneStateMachine()
    {
        Console.WriteLine("~PlaneStateMachine xox");
    }

    private async Task InitializeStateMachine(string id)
    {
        _triggerWithSpeedParameter = _stateMachine.SetTriggerParameters<int>(PlaneAction.Stop);
        using var scope = _serviceScopeFactory.CreateScope();
        var planeStateRepository = scope.ServiceProvider.GetRequiredService<IPlaneStateRepository>();
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
    
    private PlaneState GetState()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var planeStateRepository = scope.ServiceProvider.GetRequiredService<IPlaneStateRepository>();
        var planeEntity = planeStateRepository.GetById(Id);
        return planeEntity?.State ?? PlaneState.Stopped;
    }
    
    private void SaveState(PlaneState state)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var planeStateRepository = scope.ServiceProvider.GetRequiredService<IPlaneStateRepository>();
        var plane = new PlaneEntity()
        {
            Id = Id, Speed = CurrentSpeed, State = state
        };

        planeStateRepository.Save(plane);
    }

    private int UpdateSpeed(string planeId, int newSpeed)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var planeStateRepository = scope.ServiceProvider.GetRequiredService<IPlaneStateRepository>();
        var planeEntity = planeStateRepository.GetById(planeId);
        if(planeEntity == null) throw new ApplicationException("Plane not found");
        planeEntity.Speed = newSpeed;

        planeStateRepository.Save(planeEntity);
        CurrentSpeed = newSpeed; // TODO: this duplicates the state, should be removed
        return planeEntity.Speed;
    }

    public void TakeAction(string actionString)
    {
        Enum.TryParse<PlaneAction>(actionString, out var action);

        switch (action)
        {
            case PlaneAction.Stop:
                _stateMachine.Fire(_triggerWithSpeedParameter, CurrentSpeed);
                return;

            case PlaneAction.Start:
                _stateMachine.Fire(PlaneAction.Start);
                return;

            case PlaneAction.Accelerate:
                var updatedSpeed1 = UpdateSpeed(Id, CurrentSpeed + 35);
                PlaneStateProcessor.Process(updatedSpeed1);
                _stateMachine.Fire(PlaneAction.Accelerate);
                return;

            case PlaneAction.Decelerate:
                var updatedSpeed2 = UpdateSpeed(Id, Max(CurrentSpeed - 35, 0));
                PlaneStateProcessor.Process(updatedSpeed2);
                _stateMachine.Fire(PlaneAction.Decelerate);
                return;

            case PlaneAction.Fly:
                _stateMachine.Fire(_triggerWithSpeedParameter, CurrentSpeed);
                return;

            case PlaneAction.Land:
                _stateMachine.Fire(PlaneAction.Land);
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(action), action,
                    $"{nameof(PlaneStateMachine)} does not support {action}");
        }
    }

    public static void PrintState(StateMachine<PlaneState, PlaneAction>.Transition state, bool isOnEntry = true)
    {
        var onEntry = isOnEntry ? "OnEntry" : "OnExit";
        Console.WriteLine(
            $"\n\t{onEntry}, " +
            $"\n\tState Source : {state.Source}, " +
            $"State Trigger : {state.Trigger}, " +
            $"State destination : {state.Destination}");
    }
}