using System.Threading.Channels;
using Stateless;

namespace StateMachine.VehicleStateMachines;

public static class PlaneStateMachineConfigurator
{
    public static void ConfigureStates(
        StateMachine<PlaneStateMachine.PlaneState, PlaneStateMachine.PlaneAction> stateMachine,
        StateMachine<PlaneStateMachine.PlaneState, PlaneStateMachine.PlaneAction>.TriggerWithParameters<int>
            triggerWithSpeedParameter)
    {
        stateMachine.Configure(PlaneStateMachine.PlaneState.Stopped)
            .Permit(PlaneStateMachine.PlaneAction.Start, PlaneStateMachine.PlaneState.Started)
            .OnEntry((state) => { PlaneStateMachine.PrintState(state); })
            .OnExit(state => PlaneStateMachine.PrintState(state, isOnEntry: false));

        stateMachine.Configure(PlaneStateMachine.PlaneState.Started)
            .Permit(PlaneStateMachine.PlaneAction.Accelerate, PlaneStateMachine.PlaneState.Running)
            .Permit(PlaneStateMachine.PlaneAction.Stop, PlaneStateMachine.PlaneState.Stopped)
            .OnEntry((state) => { PlaneStateMachine.PrintState(state); })
            .OnExit(state => PlaneStateMachine.PrintState(state, isOnEntry: false));

        stateMachine.Configure(PlaneStateMachine.PlaneState.Running)
            .OnEntry((transition) =>
            {
                Console.WriteLine("Running...");
            })
            .PermitIf(triggerWithSpeedParameter, PlaneStateMachine.PlaneState.Stopped,
                (speed) => speed.Equals(0))
            .InternalTransition(PlaneStateMachine.PlaneAction.Accelerate,
                () => { Console.WriteLine("Accelerating..."); })
            .InternalTransition(PlaneStateMachine.PlaneAction.Decelerate,
                () => { Console.WriteLine("decelerating..."); })
            /*
            // .PermitIf(PlaneStateMachine.PlaneAction.Stop, PlaneStateMachine.PlaneState.Stopped, () => CurrentSpeed == 0)
            // .PermitIf(PlaneStateMachine.PlaneAction.Fly, PlaneStateMachine.PlaneState.Flying, () => CurrentSpeed > 100)
            */
            .OnExit(state => PlaneStateMachine.PrintState(state, isOnEntry: false));
        /*

        stateMachine.Configure(PlaneStateMachine.PlaneState.Flying)
            .Permit(PlaneStateMachine.PlaneAction.Land, PlaneStateMachine.PlaneState.Running)
            .OnExit(state => PlaneStateMachine.PrintState(state, false));
            */
    }
}