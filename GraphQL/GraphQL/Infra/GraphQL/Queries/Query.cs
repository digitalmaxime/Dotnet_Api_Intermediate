using GraphQL.Domain.Entities;
using GraphQL.Persistence.Context;
using Microsoft.AspNetCore.Mvc;

namespace GraphQL.Infra.GraphQL.Queries;

public class Query
{
    // HotChocolate will inject the DemoDbContext instance into the GetPlatform method
    public IQueryable<Platform> GetPlatform([Service(ServiceKind.Synchronized)] DemoDbContext context)
    {
        return context.Platforms;
    }
}