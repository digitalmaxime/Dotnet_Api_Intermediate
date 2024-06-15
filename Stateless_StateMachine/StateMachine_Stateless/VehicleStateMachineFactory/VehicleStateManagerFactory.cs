using StateMachine.Persistence;
using StateMachine.VehicleStateMachines;

namespace StateMachine.VehicleStateMachineFactory;

public class VehicleFactory : IVehicleFactory
{
    private readonly ICarStateRepository _carStateRepository;
    private readonly IPlaneStateRepository _planeStateRepository;
    public Dictionary<string, CarStateMachine> CarStateMachineDictionary = new();
    public Dictionary<string, PlaneStateMachine> PlaneStateMachineDictionary = new();

    public VehicleFactory(ICarStateRepository carStateRepository, IPlaneStateRepository planeStateRepository)
    {
        _carStateRepository = carStateRepository;
        _planeStateRepository = planeStateRepository;
    }

    private CarStateMachine GetOrAddCarStateMachine(string id)
    {
        var success = CarStateMachineDictionary.TryGetValue(id, out var stateMachine);
        if (!success)
        {
            stateMachine = new CarStateMachine(id, _carStateRepository);
            CarStateMachineDictionary.Add(id, stateMachine);
        }

        return stateMachine;
    }

    private PlaneStateMachine GetOrAddPlaneStateMachine(string id)
    {
        var success = PlaneStateMachineDictionary.TryGetValue(id, out var stateMachine);
        if (!success)
        {
            stateMachine = new PlaneStateMachine(id, _planeStateRepository);
            PlaneStateMachineDictionary.Add(id, stateMachine);
        }

        return stateMachine;
    }

    public IVehicleStateMachine CreateVehicleStateMachine(VehicleType type, string vehicleId)
    {
        return type switch
        {
            VehicleType.Car => GetOrAddCarStateMachine(vehicleId),
            VehicleType.Plane => GetOrAddPlaneStateMachine(vehicleId),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}