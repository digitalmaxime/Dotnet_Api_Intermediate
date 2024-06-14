using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence;

public interface IVehicleStateRepository
{
    void Save(string name, State state, int speed);

    VehicleEntity? GetByName(string name);
}

public class VehicleStateRepository : IVehicleStateRepository
{
    private readonly CarStateDbContext _dbContext;

    public VehicleStateRepository(CarStateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Save(string name, State state, int speed)
    {
        var carEntity = _dbContext.CarEntity.FirstOrDefault(c => c.Name == name);
        
        if (carEntity == null) return;
        
        carEntity.State = state;
        carEntity.Speed = speed;
        
        _dbContext.SaveChanges();
    }

    public VehicleEntity? GetByName(string name)
    {
        return _dbContext.CarEntity.FirstOrDefault(c => c.Name == name);
    }
}