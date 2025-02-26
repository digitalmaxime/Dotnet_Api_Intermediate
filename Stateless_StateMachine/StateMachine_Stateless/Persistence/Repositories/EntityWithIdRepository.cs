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
        var vehicleEntity =
            await _dbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == entity.Id);
            // _dbContext.Set<T>().Update(entity);
            _dbContext.Entry(vehicleEntity ?? throw new InvalidOperationException())
                .CurrentValues.SetValues(entity);
        await _dbContext.SaveChangesAsync();
    }

    public T? GetById(string id)
    {
        return _dbContext.Set<T>().AsNoTracking().FirstOrDefault(c => c.Id == id);
    }
}