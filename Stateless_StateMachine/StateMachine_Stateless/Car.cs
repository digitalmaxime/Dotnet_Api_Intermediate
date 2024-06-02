using Car_States_Actions;
using Stateless;
using Action = Car_States_Actions.Action;

namespace Car_StateMachine;

public static class Car
{
    public static State ExternalStateStorage = State.Stopped; // Taken from database

    public static StateMachine<State, Action>.TriggerWithParameters<int>? AccelerateWithParam;

    public static readonly StateMachine<State, Action> CarState = new(
        () => ExternalStateStorage,
        (state) => ExternalStateStorage = state);

    public static void ConfigureCarStates()
    {
        AccelerateWithParam = CarState?.SetTriggerParameters<int>(Action.Accelerate);
        
        CarState.Configure(State.Stopped)
            .Permit(Action.Start, State.Started)
            .Ignore(Action.Stop) // --> same as .PermitReentry(Action.Stop)
            .OnEntry(state =>
                Console.WriteLine(
                    $"OnEntry\n State Source : {state.Source}, State Trigger : {state.Trigger}, State destination : {state.Destination}"))
            .OnExit(state =>
                Console.WriteLine(
                    $"OnExit\n State Source : {state.Source}, State Trigger : {state.Trigger}, State destination : {state.Destination}"))
            ;

        CarState.Configure(State.Started)
            .Permit(Action.Accelerate, State.Running)
            .Permit(Action.Stop, State.Stopped)
            .OnEntry(state =>
                Console.WriteLine(
                    $"OnEntry\n State Source : {state.Source}, State Trigger : {state.Trigger}, State destination : {state.Destination}"))
            .OnExit(state =>
                Console.WriteLine(
                    $"OnExit\n State Source : {state.Source}, State Trigger : {state.Trigger}, State destination : {state.Destination}"))
            ;

        CarState.Configure(State.Running)
            .SubstateOf(State.Started)
            .OnEntryFrom(AccelerateWithParam, speed => Console.WriteLine($"Speed is {speed}"))
            .Permit(Action.Stop, State.Stopped)
            .InternalTransition(Action.Start, () => Console.WriteLine("Entered 'Start' while Running"));
    }
}