namespace StateMachine.VehicleStateMachines;

public partial class PlaneStateMachine
{
    public enum PlaneAction
    {
        Stop,
        Start,
        Accelerate,
        Decelerate,
        Fly,
        Land
    }
}