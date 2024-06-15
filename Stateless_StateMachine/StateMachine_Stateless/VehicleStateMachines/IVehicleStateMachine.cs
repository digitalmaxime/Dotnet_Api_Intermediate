namespace StateMachine.VehicleStateMachines;

public interface IVehicleStateMachine
{
    string Id { get; }
    IEnumerable<string> GetPermittedTriggers { get; }
    string GetCurrentState { get; }
    void TakeAction(string carAction);
}