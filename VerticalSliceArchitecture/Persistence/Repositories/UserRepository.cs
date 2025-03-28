using Application.Contracts.Persistence;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Repositories;

public class UserRepository(UserDbContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetUserList(int page, int pageSize)
    {

        return await context.Users
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public IQueryable<User> GetQueryableUsers()
    {
        return context.Users.AsQueryable();
    }
}