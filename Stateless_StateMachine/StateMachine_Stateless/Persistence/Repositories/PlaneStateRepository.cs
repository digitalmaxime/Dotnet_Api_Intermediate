using StateMachine.Persistence.Contracts;
using StateMachine.Persistence.Domain;

namespace StateMachine.Persistence.Repositories;

public class PlaneStateRepository : EntityWithIdRepository<PlaneEntity>, IPlaneStateRepository
{
    public PlaneStateRepository(VehicleDbContext dbContext): base(dbContext)
    {
    }
}