using Microsoft.EntityFrameworkCore;
using StateMachine.Persistence.Contracts;
using StateMachine.Persistence.Domain;

namespace StateMachine.Persistence.Repositories;

public class EntityWithIdRepository<T> : IEntityWithIdRepository<T> where T : EntityWithId
{
    private readonly VehicleDbContext _dbContext;

    protected EntityWithIdRepository(VehicleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Save(T entity)
    {
        var id = entity.Id;
        var vehicleEntity =
            await _dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (vehicleEntity == null)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }
        else
        {
            _dbContext.Set<T>().Update(entity);
        }

        await _dbContext.SaveChangesAsync();
    }

    public T? GetById(string id)
    {
        return _dbContext.Set<T>().AsNoTracking().FirstOrDefault(c => c.Id == id);
    }
}