using GraphQL.Domain.Entities;
using GraphQL.Infra.GraphQL.Commands;
using GraphQL.Infra.GraphQL.Platforms;
using GraphQL.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.Infra.GraphQL;

public class MyQuery
{
    // HotChocolate will inject the DemoDbContext instance into the GetPlatform method
    // [UseFiltering] // Implementation first style
    public IQueryable<Platform?> GetPlatform([Service(ServiceKind.Synchronized)] DemoDbContext context)
    {
        return context.Platforms
            // .Include(p => p.Commands) -- no need with Resolvers, because with inject dbContext to get those specifically
            .AsNoTracking();
    }
    
    public IQueryable<Command> GetCommands([Service(ServiceKind.Synchronized)] DemoDbContext context)
    {
        return context.Commands
            // .Include(c => c.Platform) -- no need with Resolvers, because with inject dbContext to get those specifically
            .AsNoTracking();
    }
}

public class QueryType : ObjectType<MyQuery>
{
    protected override void Configure(IObjectTypeDescriptor<MyQuery> descriptor)
    {
        descriptor.Field(q => q.GetPlatform(default!))
            .Type<ListType<PlatformType>>()
            .Name("platforms")
            .Description("Represents the list of available platforms")
            .UseFiltering();

        descriptor.Field(q => q.GetCommands(default!))
            .Type<ListType<CommandType>>()
            .Name("commands")
            .Description("Represents the list of available commands");
    }
}