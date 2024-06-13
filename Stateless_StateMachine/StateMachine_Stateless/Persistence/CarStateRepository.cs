namespace CarStateMachine.Persistence;

public interface ICarStateRepository
{
    void Save(string name, Car.State state, int speed);

    CarEntity? Get(string name);
}

public class CarStateRepository : ICarStateRepository
{
    private readonly CarStateDbContext _dbContext;

    public CarStateRepository(CarStateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Save(string name, Car.State state, int speed)
    {
        var carEntity = _dbContext.CarEntity.FirstOrDefault(c => c.Name == name);
        
        if (carEntity == null) return;
        
        carEntity.State = state;
        carEntity.Speed = speed;
        
        _dbContext.SaveChanges();
    }

    public CarEntity? Get(string name)
    {
        return _dbContext.CarEntity.FirstOrDefault(c => c.Name == name);
    }
}