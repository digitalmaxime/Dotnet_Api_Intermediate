namespace CarStateMachine.CarStateManager;

public interface IVehicleStateManager
{
    public void ProcessInputTrigger(Action action, IVehicleStateMachineBase carStateMachine);
}