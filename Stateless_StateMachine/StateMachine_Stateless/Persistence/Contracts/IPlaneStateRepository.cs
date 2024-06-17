using StateMachine.Persistence.Domain;

namespace StateMachine.Persistence.Contracts;

public interface IPlaneStateRepository : IEntityWithIdRepository<PlaneEntity>
{
}