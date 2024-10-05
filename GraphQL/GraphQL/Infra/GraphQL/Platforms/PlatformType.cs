using GraphQL.Domain.Entities;
using GraphQL.Persistence.Context;

namespace GraphQL.Infra.GraphQL.Platforms;

public class PlatformType : ObjectType<Platform>
{
    protected override void Configure(IObjectTypeDescriptor<Platform> descriptor)
    {
        descriptor.Description("Represents the platform's unique identifier");

        descriptor
            .Field(p => p.Id)
            .Type<NonNullType<IntType>>();

        descriptor
            .Field(p => p.Name)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(p => p.Description)
            .Type<StringType>();

        descriptor
            .Field(p => p.LiscenceKey)
            .Ignore();

        descriptor
            .Field(p => p.Commands)
            .ResolveWith<Resolvers>(r => r.GetCommands(default!, default!))
            .Description("The list of command line available within this platform")
            ;
    }

    private sealed class Resolvers
    {
        public IQueryable<CommandSubset> GetCommands([Parent] Platform arg,
            [Service(ServiceKind.Synchronized)] DemoDbContext context)
        {
            return context.Commands
                .Where(c => c.PlatformId == arg.Id)
                .Select(x => new CommandSubset()
                {
                    CommandLine = x.CommandLine,
                });
        }
    }

    public sealed class CommandSubset
    {
        public string CommandLine { get; set; } = default!;
    }
}