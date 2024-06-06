using Car_States_Actions;
using Stateless;
using Action = Car_States_Actions.Action;

namespace Car_StateMachine;

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
            .Ignore(Action.Stop) // --> same as .PermitReentry(Action.Stop)
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
            .InternalTransition<int>(AccelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            .InternalTransition<int>(DecelerateWithParam, (speed, _) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            ;

        CarState?.Configure(State.Speeding)
            .OnEntryFrom(AccelerateWithParam, (speed) =>
            {
                CurrentSpeed = speed;
                Console.WriteLine($"\tSpeed is {speed}");
            })
            .Permit(Action.Decelerate, State.Running)
            .Permit(Action.Fly, State.Flying)
            .InternalTransition(Action.Fly,
                () => Console.WriteLine("\tInternalTransition Fly, qu'est-ce que c'est? while Speeding"));

        CarState?.Configure(State.Flying)
            .Permit(Action.Land, State.Speeding)
            .InternalTransition(Action.Land,
                () => Console.WriteLine("\tInternalTransition Land, qu'est-ce que c'est? while Flying"));
    }
}