using Microsoft.Extensions.DependencyInjection;
using Stateless;
using Stateless.Reflection;
using StateMachine.Application;
using StateMachine.Persistence.Contracts;
using StateMachine.Persistence.Domain;
using static System.Int32;

namespace StateMachine.VehicleStateMachines;

public partial class PlaneStateMachine : IPlaneStateMachine
{
    private readonly IPlaneStateProcessor _planeStateProcessor;
    // private readonly IPlaneStateRepository _planeStateRepository;

    private StateMachine<PlaneState, PlaneAction>? _stateMachine;
    public string GetCurrentState => _stateMachine?.State.ToString() ?? "Not configured";

    public IEnumerable<string>? GetPermittedTriggers => _stateMachine?.GetPermittedTriggers()
        .Select(x => x.ToString());

    private StateMachine<PlaneState, PlaneAction>.TriggerWithParameters<int>? _stopTriggerWithSpeedParameter;
    private StateMachine<PlaneState, PlaneAction>.TriggerWithParameters<int>? _flyTriggerWithSpeedParameter;

    public PlaneStateMachine(IPlaneStateProcessor planeStateProcessor, IPlaneStateRepository planeStateRepository)
    {
        _planeStateProcessor = planeStateProcessor;
        // _planeStateRepository = planeStateRepository;
    }

    ~PlaneStateMachine()
    {
        Console.WriteLine("~PlaneStateMachine xox");
    }

    public void ConfigureStateMachine(string vehicleId)
    {
        _stateMachine = new StateMachine<PlaneState, PlaneAction>(
            () => _planeStateProcessor.GetState(vehicleId),
            (planeState) => _planeStateProcessor.SaveState(vehicleId, planeState));
        _stopTriggerWithSpeedParameter = _stateMachine.SetTriggerParameters<int>(PlaneAction.Stop);
        _flyTriggerWithSpeedParameter = _stateMachine.SetTriggerParameters<int>(PlaneAction.Fly);
        PlaneStateMachineConfigurator.ConfigureStates(_stateMachine, _stopTriggerWithSpeedParameter, _flyTriggerWithSpeedParameter);
    }

    // private PlaneState GetState(string planeId)
    // {
    //     var planeEntity = _planeStateRepository.GetById(planeId);
    //     return planeEntity?.State ?? PlaneState.Stopped;
    // }
    //
    // private void SaveState(string planeId, PlaneState state)
    // {
    //     var plane = _planeStateRepository.GetById(planeId);
    //
    //     if (plane == null) return;
    //
    //     plane.State = state;
    //
    //     _planeStateRepository.Save(plane);
    // }
    
    public void TakeAction(string vehicleId, string actionString)
    {
        Enum.TryParse<PlaneAction>(actionString, out var action);
        var speed = _planeStateProcessor.GetSpeed(vehicleId);

        switch (action)
        {
            case PlaneAction.Stop:
                _stateMachine?.Fire(_stopTriggerWithSpeedParameter, speed);
                return;

            case PlaneAction.Start:
                _stateMachine?.Fire(PlaneAction.Start);
                return;

            case PlaneAction.Accelerate:
                var updatedSpeed1 = _planeStateProcessor.UpdateSpeed(vehicleId, speed + 35);
                PlaneStateProcessor.Process(updatedSpeed1);
                _stateMachine?.Fire(PlaneAction.Accelerate);
                return;

            case PlaneAction.Decelerate:
                var updatedSpeed2 = _planeStateProcessor.UpdateSpeed(vehicleId, Max(speed - 35, 0));
                PlaneStateProcessor.Process(updatedSpeed2);
                _stateMachine?.Fire(PlaneAction.Decelerate);
                return;

            case PlaneAction.Fly:
                _stateMachine?.Fire(_flyTriggerWithSpeedParameter, speed);
                return;

            case PlaneAction.Land:
                _stateMachine?.Fire(PlaneAction.Land);
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

public interface IPlaneStateMachine
{
    IEnumerable<string>? GetPermittedTriggers { get; }
    string GetCurrentState { get; }
    void TakeAction(string vehicleId, string actionString);
    void ConfigureStateMachine(string vehicleId);
}