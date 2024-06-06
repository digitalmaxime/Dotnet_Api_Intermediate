using System.Text.Json;
using CarStateMachine.CarStateManagerFactory;

namespace Car_StateMachine;

public class Game
{
    private readonly ICarStateManagerFactory _createManagerFactory;

    public Game(ICarStateManagerFactory createManagerFactory)
    {
        _createManagerFactory = createManagerFactory;
    }
    
    public void Start(string type)
    {
        Car.ConfigureCarStates();

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