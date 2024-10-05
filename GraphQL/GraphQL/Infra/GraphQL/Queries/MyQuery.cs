using GraphQL.Domain.Entities;
using GraphQL.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.Infra.GraphQL.Queries;

public class MyQuery
{
    // HotChocolate will inject the DemoDbContext instance into the GetPlatform method
    public IQueryable<Platform> GetPlatform([Service(ServiceKind.Synchronized)] DemoDbContext context)
    {
        return context.Platforms
            // .Include(p => p.Commands) -- no need with Resolvers apparently
            .AsNoTracking();
    }
    
    public IQueryable<Command> GetCommands([Service(ServiceKind.Synchronized)] DemoDbContext context)
    {
        return context.Commands
            .Include(c => c.Platform)
            .AsNoTracking();
    }
}