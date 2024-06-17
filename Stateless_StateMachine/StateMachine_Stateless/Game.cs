using System.Text.Json;
using StateMachine.VehicleStateMachineFactory;

namespace StateMachine;

public class Game
{
    private readonly IVehicleFactory _vehicleFactory;

    public Game(IVehicleFactory vehicleFactory)
    {
        _vehicleFactory = vehicleFactory;
    }

    public void Start(VehicleType type, string vehicleId)
    {
        var stateMachine = _vehicleFactory.CreateVehicleStateMachine(type, vehicleId);

        string? input;
        do
        {
            Console.Write($"Current car {stateMachine.Id} at state : {stateMachine.GetCurrentState}\n" +
                          $"\tChoices : {JsonSerializer.Serialize(stateMachine.GetPermittedTriggers)} or 'q' to quit : ");

            input = Console.ReadLine();
            if (input == "q") break;
            if (input == "" && stateMachine.GetPermittedTriggers.Contains("Start")) input = "Start";

            try
            {
                if (input != null) stateMachine.TakeAction(input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } while (input != "q");

        Console.WriteLine("Game ended");
    }
}