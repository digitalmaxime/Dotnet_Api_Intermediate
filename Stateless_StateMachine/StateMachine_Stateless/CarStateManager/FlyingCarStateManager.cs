// using static System.Int32;
//
// namespace CarStateMachine.CarStateManager;
//
// public class FlyingCarStateManager: CarStateManagerBase
// {
//     protected override void ProcessInputTrigger(CarStateMachine.Action action, int speed, CarStateMachine carStateMachine)
//     {
//         switch (action)
//         {
//             case CarStateMachine.Action.Stop:
//                 carStateMachine.Stop();
//                 break;
//
//             case CarStateMachine.Action.Start:
//                 carStateMachine.Start();
//                 break;
//
//             case CarStateMachine.Action.Accelerate:
//                 carStateMachine.Accelerate(speed);
//                 break;
//
//             case CarStateMachine.Action.Decelerate:
//                 carStateMachine.Decelerate(Max(speed, 0));
//                 break;
//
//             case CarStateMachine.Action.Fly:
//                 carStateMachine.Fly();
//                 break;
//
//             case CarStateMachine.Action.Land:
//                 carStateMachine.Land();
//                 break;
//
//             default:
//                 throw new ArgumentOutOfRangeException();
//         }
//     }
// }