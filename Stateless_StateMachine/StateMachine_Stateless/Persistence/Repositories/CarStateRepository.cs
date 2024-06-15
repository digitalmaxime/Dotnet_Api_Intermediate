// using StateMachine.Persistence.Domain;
// using StateMachine.VehicleStateMachines;
//
// namespace StateMachine.Persistence.Repositories;
//
// public interface ICarStateRepository
// {
//     void Save(string id, CarStateMachine.CarState state, int speed);
//
//     CarEntity? GetById(string id);
// }
//
// public class CarStateRepository : ICarStateRepository
// {
//     private readonly VehicleDbContext _dbContext;
//
//     public CarStateRepository(VehicleDbContext dbContext)
//     {
//         _dbContext = dbContext;
//     }
//
//     public void Save(string id, CarStateMachine.CarState state, int speed)
//     {
//         var carEntity = _dbContext.CarEntity.FirstOrDefault(c => c.Id == id);
//
//         if (carEntity == null)
//         {
//             _dbContext.Add(new CarEntity()
//             {
//                 Id = id, State = state, Speed = speed
//             });
//         }
//         else
//         {
//             carEntity.State = state;
//             carEntity.Speed = speed;
//         }
//
//         _dbContext.SaveChanges();
//     }
//
//     public CarEntity? GetById(string id)
//     {
//         return _dbContext.CarEntity.FirstOrDefault(c => c.Id == id);
//     }
// }