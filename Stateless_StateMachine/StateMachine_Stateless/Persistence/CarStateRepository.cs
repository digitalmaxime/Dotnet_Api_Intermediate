using Stateless.Graph;
using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence;

public interface ICarStateRepository
{
    void Save(string name, CarStateMachine.CarState state, int speed);

    CarEntity? GetById(string id);
}

public class CarStateRepository : ICarStateRepository
{
    private readonly CarStateDbContext _dbContext;

    public CarStateRepository(CarStateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Save(string name, CarStateMachine.CarState state, int speed)
    {
        var carEntity = _dbContext.CarEntity.FirstOrDefault(c => c.Id == name);
        
        if (carEntity == null) return;
        
        carEntity.State = state;
        carEntity.Speed = speed;
        
        _dbContext.SaveChanges();
    }

    public CarEntity? GetById(string id)
    {
        return _dbContext.CarEntity.FirstOrDefault(c => c.Id == id);
    }
}