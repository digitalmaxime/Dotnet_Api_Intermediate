using System.Diagnostics;
using System.Text.Json;
using CarStateMachine.CarStateManager;
using CarStateMachine.CarStateManagerFactory;
using CarStateMachine.Persistence;

namespace CarStateMachine;

public class Game
{
    private readonly IVehicleStateMachineBase _carStateMachine;
    private readonly IVehicleStateManagerFactory _createManagerFactory;
    private readonly IVehicleStateRepository _vehicleStateRepository;

    public Game(IVehicleStateMachineBase carStateMachine, IVehicleStateManagerFactory createManagerFactory,
        IVehicleStateRepository vehicleStateRepository)
    {
        _carStateMachine = carStateMachine;
        _createManagerFactory = createManagerFactory;
        _vehicleStateRepository = vehicleStateRepository;
    }

    public void Start(VehicleType type, string vehicleName)
    {
        var vehicleStateManager = _createManagerFactory.GetCarStateManager(type);

        var carEntity = _vehicleStateRepository.GetByName(vehicleName);

        Debug.Assert(carEntity != null, nameof(carEntity) + " != null");

        string? rawInput;
        do
        {
            Console.WriteLine($"Current car state : {_carStateMachine.CurrentState} " +
                              $"\tChoices : {JsonSerializer.Serialize(_carStateMachine.PermittedTriggers)}");

            rawInput = Console.ReadLine();

            try
            {
                var actionInput = VehicleStateManagerHelper.ValidateUserInput(rawInput);
                vehicleStateManager.ProcessInputTrigger(actionInput, _carStateMachine);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        } while (rawInput != "q");

        Console.WriteLine("Bye bye");
    }
}