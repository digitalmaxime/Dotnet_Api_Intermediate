namespace CarStateMachine.Persistence;

public interface ICarStateRepository
{
    void Save(CarStateMachine? carState);
    
    CarStateMachine? Get(string name);
}

public class CarStateRepository : ICarStateRepository
{
    private readonly CarStateDbContext _dbContext;

    public CarStateRepository(CarStateDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void Save(CarStateMachine? carState)
    {
        _dbContext.Car.Add(carState);
        _dbContext.SaveChanges();
    }

    public CarStateMachine? Get(string name)
    {
        return _dbContext.Car.FirstOrDefault(c => c.Name == name);
    }
}