namespace StateMachine.VehicleStateMachines;

public interface IVehicleStateMachine
{
    string Id { get; set; }
    IEnumerable<string> GetPermittedTriggers { get; }
    string GetCurrentState { get; }
    void TakeAction(string carAction);
}