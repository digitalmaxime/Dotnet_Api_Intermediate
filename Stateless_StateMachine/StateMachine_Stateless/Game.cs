using System.Diagnostics;
using System.Text.Json;
using StateMachine.InputManagement;
using StateMachine.Persistence;
using StateMachine.VehicleStateMachineFactory;

namespace StateMachine;

public class Game
{
    // private readonly ICarStateRepository _carStateRepository;
    private readonly IVehicleFactory _vehicleFactory;

    public Game(ICarStateRepository carStateRepository, IVehicleFactory vehicleFactory)
    {
        // _carStateRepository = carStateRepository;
        _vehicleFactory = vehicleFactory;
    }

    public void Start(VehicleType type, string vehicleId)
    {
        // var vehicleEntity = _carStateRepository.GetById(vehicleId);

        // if (vehicleEntity == null)
        // {
        //     Console.WriteLine("No vehicle found.... :(");
        //     return;
        // }

        var stateMachine = _vehicleFactory.CreateVehicleStateMachine(type, vehicleId); 

        Debug.Assert(vehicleEntity != null, nameof(vehicleEntity) + " != null");

        string? rawInput;
        do
        {
            Console.Write($"Current car {stateMachine.Id} at state : {stateMachine.CurrentState} \n" +
                              $"\tChoices : {JsonSerializer.Serialize(stateMachine.PermittedTriggers.Select(x => x.ToString()))} or 'q' to quit : ");

            rawInput = Console.ReadLine();
            if (rawInput == "q") break;

            try
            {
                var actionInput = VehicleStateManagerHelper.ValidateUserInput(rawInput);
                stateMachine.TakeAction(actionInput);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } while (rawInput != "q");

        Console.WriteLine("Game ended");
    }
}