using StateMachine.Persistence;
using static System.Int32;

namespace StateMachine.VehicleStateMachines;

public class PlaneStateMachine : IVehicleStateMachine
{
    public enum PlaneState
    {
        Stopped,
        Started,
        Running,
        Drifting,
        Flying,
    }

    public enum PlaneAction
    {
        Stop,
        Start,
        Accelerate,
        Decelerate,
        Drift,
        Fly,
        Land
    }
    public int Altitude { get; set; }

    public PlaneStateMachine(string id, IPlaneStateRepository carStateRepository)
    {
        Id = id;
        _planeStateRepository = carStateRepository;
        InitializeStateMachine(id);
        ConfigureStates();
    }
    ~PlaneStateMachine()
    {
        Console.WriteLine("~PlaneStateMachine xox");
    }
    
    private void InitializeStateMachine(string id)
    {
        var car = _planeStateRepository.GetById(id);
        if (car == null)
        {
            // TODO: Create
        }

        CurrentPlaneState = car?.State ?? PlaneState.Stopped;
        CurrentSpeed = car?.Speed ?? -1;

        _stateMachine = new StateMachine<PlaneState, PlaneAction>(
            () => CurrentCarState,
            (s) =>
            {
                CurrentCarState = s;
                SaveState();
            }
        );
    }
    protected void ConfigureStates()
    {
        AccelerateWithParam = StateMachine.SetTriggerParameters<int>(PlaneAction.Accelerate);
        DecelerateWithParam = StateMachine.SetTriggerParameters<int>(PlaneAction.Decelerate);
        
        StateMachine.Configure(PlaneState.Stopped)
            .Permit(PlaneAction.Start, PlaneState.Started)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState);

        StateMachine.Configure(PlaneState.Started)
            .Permit(PlaneAction.Accelerate, PlaneState.Running)
            .Permit(PlaneAction.Stop, PlaneState.Stopped)
            .OnEntry((state) =>
            {
                CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState);
        StateMachine.Configure(PlaneState.Running)
            .OnEntryFrom(AccelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                SaveState();
                Console.WriteLine($"\tSpeed is {CurrentSpeed}");
            })
            .PermitIf(PlaneAction.Stop, PlaneState.Stopped, () => CurrentSpeed == 0)
            .PermitIf(PlaneAction.Fly, PlaneState.Flying, () => CurrentSpeed > 100)
            .InternalTransition<int>(AccelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                SaveState();
                Console.WriteLine($"\tSpeed is {CurrentSpeed}");
            })
            .InternalTransitionIf<int>(DecelerateWithParam, _ => CurrentSpeed > 0, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {CurrentSpeed}");
            });

        StateMachine.Configure(PlaneState.Flying)
            .Permit(PlaneAction.Land, PlaneState.Running);
    }

    public void TakeAction(PlaneAction action)
    {
        switch (action)
        {
            case PlaneAction.Stop:
                StateMachine.Fire(PlaneAction.Stop);
                return;

            case PlaneAction.Start:
                StateMachine.Fire(PlaneAction.Start);
                return;

            case PlaneAction.Accelerate:
                StateMachine.Fire(AccelerateWithParam, CurrentSpeed + 35);
                return;

            case PlaneAction.Decelerate:
                StateMachine.Fire(DecelerateWithParam, Max(CurrentSpeed - 35, 0));
                return;

            case PlaneAction.Fly:
                StateMachine.Fire(PlaneAction.Fly);
                return;

            case PlaneAction.Land:
                StateMachine.Fire(PlaneAction.Land);
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(action), action,
                    $"{nameof(PlaneStateMachine)} does not support {action}");
        }
    }
}