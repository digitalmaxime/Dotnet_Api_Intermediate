using StateMachine.Persistence.Contracts;
using StateMachine.VehicleStateMachines;

namespace StateMachine.Application;

public interface IPlaneStateProcessor
{
    int UpdateSpeed(string planeId, int newSpeed);
    int GetSpeed(string planeId);
    void SaveState(string planeId, PlaneStateMachine.PlaneState state);
    PlaneStateMachine.PlaneState GetState(string planeId);
}

public class PlaneStateProcessor : IPlaneStateProcessor
{
    private readonly IPlaneStateRepository _planeStateRepository;
    
    public PlaneStateProcessor(IPlaneStateRepository planeStateRepository)
    {
        _planeStateRepository = planeStateRepository;
    }
    public static void Process(int speed)
    {
        Console.WriteLine($"Processing speed : {speed}");
    }
    
    public int GetSpeed(string planeId)
    {
        var planeEntity = _planeStateRepository.GetById(planeId);
        if (planeEntity == null) throw new ApplicationException("Plane not found");
        return planeEntity.Speed;
    }

    public int UpdateSpeed(string planeId, int newSpeed)
    {
        var planeEntity = _planeStateRepository.GetById(planeId);
        if (planeEntity == null) throw new ApplicationException("Plane not found");
        planeEntity.Speed = newSpeed;

        _planeStateRepository.Save(planeEntity);
        return planeEntity.Speed;
    }

    public PlaneStateMachine.PlaneState GetState(string planeId)
    {
        var planeEntity = _planeStateRepository.GetById(planeId);
        return planeEntity?.State ?? PlaneStateMachine.PlaneState.Stopped;
    }
    public void SaveState(string planeId, PlaneStateMachine.PlaneState state)
    {
        var plane = _planeStateRepository.GetById(planeId);

        if (plane == null) return;

        plane.State = state;

        _planeStateRepository.Save(plane);
    }

}
