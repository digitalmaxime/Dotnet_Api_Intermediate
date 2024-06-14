namespace StateMachine.InputManagement;

public static class VehicleStateManagerHelper
{
    public static Action ValidateUserInput(string? input)
    {
        var success = Enum.TryParse<Action>(input, out var action);

        if (!success && input != "q")
        {
            Console.WriteLine("\tInvalid input, doesn't match any action");
            throw new ArgumentException("Invalid input, doesn't match any action");
        }

        return action;
    }
}