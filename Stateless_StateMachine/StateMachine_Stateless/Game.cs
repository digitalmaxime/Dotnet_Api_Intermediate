using System.Diagnostics;
using System.Text.Json;
using CarStateMachine.CarStateManagerFactory;
using CarStateMachine.Persistence;

namespace CarStateMachine;

public class Game
{
    private readonly ICarStateManagerFactory _createManagerFactory;
    private readonly ICarStateRepository _carStateRepository;

    public Game(ICarStateManagerFactory createManagerFactory, ICarStateRepository carStateRepository)
    {
        _createManagerFactory = createManagerFactory;
        _carStateRepository = carStateRepository;
    }

    public void Start(CarType type)
    {
        var carStateManager = _createManagerFactory.GetCarStateManager(type);

        var carEntity = _carStateRepository.Get("Name1");

        Debug.Assert(carEntity != null, nameof(carEntity) + " != null");
        
        var car = new Car(carEntity.Name, _carStateRepository);
        
        string? input;
        do
        {
            Console.WriteLine($"Current car state : {car.CurrentState} " +
                              $"\tChoices : {JsonSerializer.Serialize(car.PermittedTriggers)}");
            input = Console.ReadLine();
            carStateManager.ProcessUserInput(input, car);
        } while (input != "q");

        Console.WriteLine("Bye bye");
    }
}