namespace StateMachine.VehicleStateMachines;

public partial class PlaneStateMachine
{
    public enum PlaneState
    {
        Stopped,
        Started,
        Running,
        Flying,
    }
}