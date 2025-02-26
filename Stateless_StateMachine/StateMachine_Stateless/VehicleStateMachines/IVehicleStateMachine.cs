using Stateless;

namespace StateMachine.VehicleStateMachines;

public interface IVehicleStateMachine
{
    // string Id { get; }
    IEnumerable<string>? GetPermittedTriggers { get; }
    string GetCurrentState { get; }
    void TakeAction(string vehicleId, string actionString);
    void ConfigureStateMachine(string vehicleId);
}