using System.Text.Json;
using CarStateMachine.CarStateManagerFactory;

namespace CarStateMachine;

public class Game
{
    private readonly ICarStateManagerFactory _createManagerFactory;

    public Game(ICarStateManagerFactory createManagerFactory)
    {
        _createManagerFactory = createManagerFactory;
        Init();
    }

    private void Init()
    {
        Car.ConfigureCarStates();
    }
    
    public void Start(CarType type)
    {
        var carStateManager = _createManagerFactory.GetCarStateManager(type);
        
        string? input;
        do
        {
            var permittedTriggers = Car.CarState.GetPermittedTriggers().Select(x => x.ToString());
            Console.WriteLine($"Current car state : {Car.ExternalStateStorage.ToString()} " +
                              $"\tChoices : {JsonSerializer.Serialize(permittedTriggers)}");
            input = Console.ReadLine();
            carStateManager.ProcessUserInput(input);
        } while (input != "q");

        Console.WriteLine("Bye bye");
    }
}