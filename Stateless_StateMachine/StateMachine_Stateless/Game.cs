using System.Text.Json;
using StateMachine.Persistence.Constants;
using StateMachine.VehicleStateMachineFactory;
using StateMachine.VehicleStateMachines;

namespace StateMachine;

public class Game
{
    // private readonly IVehicleFactory _vehicleFactory;
    private readonly IPlaneStateMachine _stateMachine;

    public Game(/*IVehicleFactory vehicleFactory,*/ IPlaneStateMachine stateMachine)
    {
        // _vehicleFactory = vehicleFactory;
        _stateMachine = stateMachine;
    }

    public void Start(VehicleType type, string vehicleId)
    {
        // var stateMachine = _vehicleFactory.CreateVehicleStateMachine(type, vehicleId);
        
        _stateMachine.ConfigureStateMachine(vehicleId);

        string? input;
        do
        {
            Console.Write($"Current car {vehicleId} at state : {_stateMachine.GetCurrentState}\n" +
                          $"\tChoices : {JsonSerializer.Serialize(_stateMachine.GetPermittedTriggers)}\n +" +
                          $" or type 'exit' to exit or type 'newVehicle' to create a new vehicle\n");
            
            input = Console.ReadLine();
            if (input == "exit") break;
            if (input == "" && _stateMachine.GetPermittedTriggers.Contains("Start")) input = "Start";

            try
            {
                if (input != null) _stateMachine.TakeAction(vehicleId, input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } while (input != "exit");

        Console.WriteLine("Game ended");
    }
}