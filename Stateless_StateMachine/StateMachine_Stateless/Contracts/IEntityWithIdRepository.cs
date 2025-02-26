using StateMachine.Persistence.Domain;

namespace StateMachine.Persistence.Contracts;

public interface IEntityWithIdRepository<T> where T : EntityWithId
{
    Task Save(T entity);
    T? GetById(string id);
}