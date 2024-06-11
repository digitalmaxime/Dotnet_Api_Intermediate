using Stateless;

namespace CarStateMachine;

public static class Car
{
    public static State ExternalStateStorage = State.Stopped; // Taken from database
    public static int CurrentSpeed { get; private set; } // Taken from database

    public static StateMachine<State, Action>.TriggerWithParameters<int>? AccelerateWithParam;
    public static StateMachine<State, Action>.TriggerWithParameters<int>? DecelerateWithParam;

    public static readonly StateMachine<State, Action> CarState = new(
        () => ExternalStateStorage,
        (state) => ExternalStateStorage = state);

    private static void PrintState(StateMachine<State, Action>.Transition state)
    {
        Console.WriteLine(
            $"\tOnExit/OnExit\n\tState Source : {state.Source}, " +
            $"State Trigger : {state.Trigger}, " +
            $"State destination : {state.Destination}");
    }

    public static void ConfigureCarStates()
    {
        AccelerateWithParam = CarState?.SetTriggerParameters<int>(Action.Accelerate);
        DecelerateWithParam = CarState?.SetTriggerParameters<int>(Action.Decelerate);

        CarState?.Configure(State.Stopped)
            .Permit(Action.Start, State.Started)
            // .Ignore(Action.Stop) // --> same as .PermitReentry(Action.Stop)
            .OnEntry((state) =>
            {
                Car.CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState)
            ;

        CarState?.Configure(State.Started)
            .Permit(Action.Accelerate, State.Running)
            .Permit(Action.Stop, State.Stopped)
            .OnEntry((state) =>
            {
                Car.CurrentSpeed = 0;
                PrintState(state);
            })
            .OnExit(PrintState)
            ;

        CarState?.Configure(State.Running)
            .OnEntryFrom(AccelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            .PermitIf(Action.Stop, State.Stopped, () => CurrentSpeed == 0)
            .PermitIf(Action.Fly, State.Flying,  () => CurrentSpeed > 100)
            .InternalTransition<int>(AccelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            .InternalTransitionIf<int>(DecelerateWithParam, (speed) => CurrentSpeed > 0, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            ;

        CarState?.Configure(State.Flying)
            .Permit(Action.Land, State.Running);
    }
}