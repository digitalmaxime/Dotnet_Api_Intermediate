using System.Diagnostics;
using System.Text.Json;
using StateMachine.InputManagement;
using StateMachine.Persistence;
using StateMachine.VehicleStateMachineFactory;

namespace StateMachine;

public class Game
{
    private readonly IVehicleStateRepository _vehicleStateRepository;
    private readonly IVehicleFactory _vehicleFactory;

    public Game( 
        IVehicleStateRepository vehicleStateRepository,
        IVehicleFactory vehicleFactory)
    {
        _vehicleStateRepository = vehicleStateRepository;
        _vehicleFactory = vehicleFactory;
    }

    public void Start(VehicleType type, string vehicleName)
    {
        var vehicleEntity = _vehicleStateRepository.GetByName(vehicleName);

        if (vehicleEntity == null)
        {
            Console.WriteLine("No vehicle found.... :(");
            return;
        }

        var stateMachine = _vehicleFactory.CreateVehicleStateMachine(type, vehicleEntity, _vehicleStateRepository); 

        Debug.Assert(vehicleEntity != null, nameof(vehicleEntity) + " != null");

        string? rawInput;
        do
        {
            Console.Write($"Current car {stateMachine.Name} at state : {stateMachine.CurrentState} \n" +
                              $"\tChoices : {JsonSerializer.Serialize(stateMachine.PermittedTriggers.Select(x => x.ToString()))} or 'q' to quit : ");

            rawInput = Console.ReadLine();
            if (rawInput == "q") break;

            try
            {
                var actionInput = VehicleStateManagerHelper.ValidateUserInput(rawInput);
                stateMachine.TakeActionBase(actionInput);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } while (rawInput != "q");

        Console.WriteLine("Game ended");
    }
}