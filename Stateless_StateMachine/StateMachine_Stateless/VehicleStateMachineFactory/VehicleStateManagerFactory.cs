using Stateless;
using Stateless.Graph;
using StateMachine.Persistence;
using StateMachine.VehicleStateMachines;

namespace StateMachine.VehicleStateMachineFactory;

public class VehicleFactory: IVehicleFactory
{
    private readonly ICarStateRepository _carStateRepository;
    public Dictionary<string, CarStateMachine> CarStateMachineDictionary { get; }
    public Dictionary<string, PlaneStateMachine> PlaneStateMachineDictionary { get; set; }

    public VehicleFactory(ICarStateRepository carStateRepository, Dictionary<string, CarStateMachine> carStateMachineDictionary)
    {
        _carStateRepository = carStateRepository;
        CarStateMachineDictionary = carStateMachineDictionary;
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
    
    public IVehicleStateMachine CreateVehicleStateMachine(VehicleType type, string vehicleId)
    {
        return type switch
        {
            VehicleType.Car => GetOrAddCarStateMachine(vehicleId),
            // VehicleType.Plane => new PlaneStateMachine(vehicleId),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}