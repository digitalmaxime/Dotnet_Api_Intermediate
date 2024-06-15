using StateMachine.Persistence.Domain;
using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence.Repositories;

public interface IEntityWithIdRepository<T> where T : EntityWithId
{
    void Save(T entity);
    T? GetById(string id);
}

public class EntityWithIdRepository<T> : IEntityWithIdRepository<T> where T : EntityWithId
{
    private readonly VehicleDbContext _dbContext;

    public EntityWithIdRepository(VehicleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Save(T entity)
    {
        var vehicleEntity = _dbContext.Set<T>().FirstOrDefault(x => x.Id == entity.Id);

        if (vehicleEntity == null)
        {
            
            _dbContext.Set<T>().Add(entity);
        }
        else
        {
            _dbContext.Set<T>().Update(entity); // TODO: valider
            // vehicleEntity.State = state;
            // vehicleEntity.Speed = speed;
        }

        _dbContext.SaveChanges();
    }

    public T? GetById(string id)
    {
        return _dbContext.Set<T>().FirstOrDefault(c => c.Id == id);
    }
}