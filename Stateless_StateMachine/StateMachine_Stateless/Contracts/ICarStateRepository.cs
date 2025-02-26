using StateMachine.Persistence.Domain;

namespace StateMachine.Persistence.Contracts;

public interface ICarStateRepository : IEntityWithIdRepository<CarEntity>
{
}