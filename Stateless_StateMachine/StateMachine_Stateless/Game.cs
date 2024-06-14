using System.Diagnostics;
using System.Text.Json;
using CarStateMachine.CarStateManagerFactory;
using CarStateMachine.Persistence;

namespace CarStateMachine;

public class Game
{
    private readonly ICarStateMachine _carStateMachine;
    private readonly ICarStateManagerFactory _createManagerFactory;
    private readonly IVehicleStateRepository _vehicleStateRepository;

    public Game(ICarStateMachine carStateMachine, ICarStateManagerFactory createManagerFactory, IVehicleStateRepository vehicleStateRepository)
    {
        _carStateMachine = carStateMachine;
        _createManagerFactory = createManagerFactory;
        _vehicleStateRepository = vehicleStateRepository;
    }

    public void Start(CarType type)
    {
        var carStateManager = _createManagerFactory.GetCarStateManager(type);

        var carEntity = _vehicleStateRepository.GetByName("Name1");

        Debug.Assert(carEntity != null, nameof(carEntity) + " != null");
                
        string? input;
        do
        {
            Console.WriteLine($"Current car state : {_carStateMachine.CurrentState} " +
                              $"\tChoices : {JsonSerializer.Serialize(_carStateMachine.PermittedTriggers)}");
            input = Console.ReadLine();
            carStateManager.ProcessUserInput(input, _carStateMachine);
        } while (input != "q");

        Console.WriteLine("Bye bye");
    }
}