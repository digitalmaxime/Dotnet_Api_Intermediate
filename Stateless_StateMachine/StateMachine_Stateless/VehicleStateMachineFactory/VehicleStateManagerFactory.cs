using StateMachine.Persistence;
using StateMachine.Persistence.Domain;
using StateMachine.Persistence.Repositories;
using StateMachine.VehicleStateMachines;

namespace StateMachine.VehicleStateMachineFactory;

public class VehicleFactory : IVehicleFactory
{
    private readonly IEntityWithIdRepository<CarEntity> _carStateRepository;
    private readonly IEntityWithIdRepository<PlaneEntity> _planeStateRepository;
    private readonly Dictionary<string, CarStateMachine> _carStateMachineDictionary = new();
    private readonly Dictionary<string, PlaneStateMachine> _planeStateMachineDictionary = new();

    public VehicleFactory(IEntityWithIdRepository<CarEntity> carStateRepository, IEntityWithIdRepository<PlaneEntity> planeStateRepository)
    {
        _carStateRepository = carStateRepository;
        _planeStateRepository = planeStateRepository;
    }

    private CarStateMachine GetOrAddCarStateMachine(string id)
    {
        var success = _carStateMachineDictionary.TryGetValue(id, out var stateMachine);
        if (!success)
        {
            stateMachine = new CarStateMachine(id, _carStateRepository);
            _carStateMachineDictionary.Add(id, stateMachine);
        }

        return stateMachine;
    }

    private PlaneStateMachine GetOrAddPlaneStateMachine(string id)
    {
        var success = _planeStateMachineDictionary.TryGetValue(id, out var stateMachine);
        if (!success)
        {
            stateMachine = new PlaneStateMachine(id, _planeStateRepository);
            _planeStateMachineDictionary.Add(id, stateMachine);
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