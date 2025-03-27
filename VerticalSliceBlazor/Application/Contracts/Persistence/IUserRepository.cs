using Application.Features.GetUserList;
using Domain;

namespace Application.Contracts.Persistence;

public interface IUserRepository
{
    public Task<IEnumerable<User>> GetUserList(int page, int pageSize);
    IQueryable<User> GetQueryableUsers();
}