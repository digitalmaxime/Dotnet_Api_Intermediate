using StateMachine.Persistence.Contracts;
using StateMachine.Persistence.Domain;

namespace StateMachine.Persistence.Repositories;

public class CarStateRepository : EntityWithIdRepository<CarEntity>, ICarStateRepository
{
    public CarStateRepository(VehicleDbContext dbContext): base(dbContext)
    {
        Console.WriteLine("CarStateRepository created");
    }
    
    ~CarStateRepository()
    {
        Console.WriteLine("CarStateRepository destroyed");
    }
}