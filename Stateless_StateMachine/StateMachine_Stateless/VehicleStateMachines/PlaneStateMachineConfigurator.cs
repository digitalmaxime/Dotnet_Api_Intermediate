using System.Threading.Channels;
using Stateless;
using StateMachine.Application;

namespace StateMachine.VehicleStateMachines;

internal static class PlaneStateMachineConfigurator
{
    public static void ConfigureStates(
        StateMachine<PlaneStateMachine.PlaneState, PlaneStateMachine.PlaneAction> stateMachine,
        StateMachine<PlaneStateMachine.PlaneState, PlaneStateMachine.PlaneAction>.TriggerWithParameters<int>
            stopTriggerWithSpeedParameter,
        StateMachine<PlaneStateMachine.PlaneState, PlaneStateMachine.PlaneAction>.TriggerWithParameters<int>
            flyTriggerWithSpeedParameter
    )
    {
        stateMachine.Configure(PlaneStateMachine.PlaneState.Stopped)
            .OnEntry((state) => PlaneStateMachine.PrintState(state))
            .Permit(PlaneStateMachine.PlaneAction.Start, PlaneStateMachine.PlaneState.Started)
            .OnExit(state => PlaneStateMachine.PrintState(state, isOnEntry: false));

        stateMachine.Configure(PlaneStateMachine.PlaneState.Started)
            .OnEntry((state) => PlaneStateMachine.PrintState(state))
            .Permit(PlaneStateMachine.PlaneAction.Accelerate, PlaneStateMachine.PlaneState.Running)
            .Permit(PlaneStateMachine.PlaneAction.Stop, PlaneStateMachine.PlaneState.Stopped)
            .OnExit(state => PlaneStateMachine.PrintState(state, isOnEntry: false));

        stateMachine.Configure(PlaneStateMachine.PlaneState.Running)
            .OnEntry(state =>
            {
                PlaneStateMachine.PrintState(state);
            })
            .PermitIf(stopTriggerWithSpeedParameter, PlaneStateMachine.PlaneState.Stopped,
                (speed) => speed.Equals(0), "Speed must be 0 to stop")
            .PermitIf(flyTriggerWithSpeedParameter, PlaneStateMachine.PlaneState.Flying,
                (speed) =>
                {
                    return speed > 100;
                }, "Speed must be greater than 100 to fly")
            .InternalTransition(PlaneStateMachine.PlaneAction.Accelerate,
                () => { Console.WriteLine("InternalTransition -- Accelerating..."); })
            .InternalTransition(PlaneStateMachine.PlaneAction.Decelerate,
                () => { Console.WriteLine("InternalTransition -- decelerating..."); })
            .OnExit(state => PlaneStateMachine.PrintState(state, isOnEntry: false));

        stateMachine.Configure(PlaneStateMachine.PlaneState.Flying)
            .OnEntry((state) => PlaneStateMachine.PrintState(state))
            .Permit(PlaneStateMachine.PlaneAction.Land, PlaneStateMachine.PlaneState.Running)
            .OnExit(state => PlaneStateMachine.PrintState(state, false));
    }
}