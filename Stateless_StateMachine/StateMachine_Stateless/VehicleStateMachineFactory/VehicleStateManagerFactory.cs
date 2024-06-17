using Microsoft.Extensions.DependencyInjection;
using StateMachine.Persistence.Constants;
using StateMachine.VehicleStateMachines;

namespace StateMachine.VehicleStateMachineFactory;

public class VehicleFactory : IVehicleFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Dictionary<string, CarStateMachine> _carStateMachineDictionary = new();
    private readonly Dictionary<string, PlaneStateMachine> _planeStateMachineDictionary = new();

    public VehicleFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    private CarStateMachine GetOrAddCarStateMachine(string id)
    {
        var success = _carStateMachineDictionary.TryGetValue(id, out var stateMachine);
        if (!success)
        {
            stateMachine = new CarStateMachine(id, _serviceScopeFactory);
            _carStateMachineDictionary.Add(id, stateMachine);
        }

        return stateMachine!;
    }

    private PlaneStateMachine GetOrAddPlaneStateMachine(string id)
    {
        var success = _planeStateMachineDictionary.TryGetValue(id, out var stateMachine);
        if (!success)
        {
            stateMachine = new PlaneStateMachine(id, _serviceScopeFactory);
            _planeStateMachineDictionary.Add(id, stateMachine);
        }

        return stateMachine!;
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