using StateMachine.Persistence.Contracts;
using StateMachine.Persistence.Domain;

namespace StateMachine.Persistence.Repositories;

public interface IPlaneStateRepository : IEntityWithIdRepository<PlaneEntity>
{
}