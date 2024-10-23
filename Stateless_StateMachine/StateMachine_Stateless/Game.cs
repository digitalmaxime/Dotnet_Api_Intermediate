using System.Text.Json;
using StateMachine.Persistence.Constants;
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
                          $"\tChoices : {JsonSerializer.Serialize(stateMachine.GetPermittedTriggers)} or 'exit' to exit : ");

            input = Console.ReadLine();
            if (input == "exit") break;
            if (input == "" && stateMachine.GetPermittedTriggers.Contains("Start")) input = "Start";

            try
            {
                if (input != null) stateMachine.TakeAction(input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } while (input != "exit");

        Console.WriteLine("Game ended");
    }
}