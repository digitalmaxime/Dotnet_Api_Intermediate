using Microsoft.Extensions.DependencyInjection;
using StateMachine.Persistence.Constants;
using StateMachine.VehicleStateMachines;

namespace StateMachine.VehicleStateMachineFactory;

public class VehicleFactory // TODO: Adjust : IVehicleFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IPlaneStateMachine _planeStateMachine;
    private readonly Dictionary<string, CarStateMachine> _carStateMachineDictionary = new();
    private readonly Dictionary<string, PlaneStateMachine> _planeStateMachineDictionary = new();

    public VehicleFactory(IServiceScopeFactory serviceScopeFactory, IPlaneStateMachine planeStateMachine)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _planeStateMachine = planeStateMachine;
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

    public IPlaneStateMachine GetOrAddPlaneStateMachine(string id)
    {
        // var success = _planeStateMachineDictionary.TryGetValue(id, out var stateMachine);
        // if (!success)
        // {
        //     stateMachine = new PlaneStateMachine(_serviceScopeFactory);
        //     _planeStateMachineDictionary.Add(id, stateMachine);
        // }

        return _planeStateMachine;
    }

    // public IVehicleStateMachine CreateVehicleStateMachine(VehicleType type, string vehicleId)
    // {
    //     return type switch
    //     {
    //         VehicleType.Car => GetOrAddCarStateMachine(vehicleId),
    //         VehicleType.Plane => GetOrAddPlaneStateMachine(vehicleId),
    //         _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    //     };
    // }
}