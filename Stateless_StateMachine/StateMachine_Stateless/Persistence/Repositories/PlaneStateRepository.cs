// using StateMachine.Persistence.Domain;
// using StateMachine.VehicleStateMachines;
//
// namespace StateMachine.Persistence.Repositories;
//
// public interface IPlaneStateRepository
// {
//     void Save(string id, PlaneStateMachine.PlaneState state, int speed);
//
//     PlaneEntity? GetById(string id);
// }
//
// public class PlaneStateRepository : IPlaneStateRepository
// {
//     private readonly VehicleDbContext _dbContext;
//
//     public PlaneStateRepository(VehicleDbContext dbContext)
//     {
//         _dbContext = dbContext;
//     }
//
//     public void Save(string id, PlaneStateMachine.PlaneState state, int speed)
//     {
//         var planeEntity = _dbContext.PlaneEntity.FirstOrDefault(c => c.Id == id);
//
//         if (planeEntity == null)
//         {
//             _dbContext.Add(new PlaneEntity()
//             {
//                 Id = id, State = state, Speed = speed
//             });
//         }
//         else
//         {
//             planeEntity.State = state;
//             planeEntity.Speed = speed;
//         }
//
//         _dbContext.SaveChanges();
//     }
//
//     public PlaneEntity? GetById(string id)
//     {
//         return _dbContext.PlaneEntity.FirstOrDefault(c => c.Id == id);
//     }
// }