using System.Text.Json;
using CarStateMachine.CarStateManagerFactory;

namespace CarStateMachine;

public class Game
{
    private readonly ICarStateManagerFactory _createManagerFactory;

    public Game(ICarStateManagerFactory createManagerFactory)
    {
        _createManagerFactory = createManagerFactory;
    }

    public void Start(CarType type)
    {
        var carStateManager = _createManagerFactory.GetCarStateManager(type);
        
        string? input;
        do
        {
            var permittedTriggers = CarStateMachine.CarState.GetPermittedTriggers().Select(x => x.ToString());
            Console.WriteLine($"Current car state : {CarStateMachine.ExternalStateStorage.ToString()} " +
                              $"\tChoices : {JsonSerializer.Serialize(permittedTriggers)}");
            input = Console.ReadLine();
            carStateManager.ProcessUserInput(input);
        } while (input != "q");

        Console.WriteLine("Bye bye");
    }
}