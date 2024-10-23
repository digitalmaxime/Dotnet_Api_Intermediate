using StateMachine.Persistence.Contracts;

namespace StateMachine.Application;

public static class PlaneStateProcessor
{
    // private readonly IPlaneStateRepository _planeStateRepository;
    //
    // public PlaneStateProcessor(IPlaneStateRepository planeStateRepository)
    // {
    //     _planeStateRepository = planeStateRepository;
    // }
    public static void Process(int speed)
    {
        Console.WriteLine($"Processing speed : {speed}");
    }
}