using GraphQL.Domain.Entities;
using GraphQL.Infra.GraphQL.Platforms;
using GraphQL.Persistence.Context;

namespace GraphQL.Infra.GraphQL.Commands;

public class CommandType : ObjectType<Command>
{
    protected override void Configure(IObjectTypeDescriptor<Command> descriptor)
    {
        descriptor.Field(c => c.Id)
            .Type<NonNullType<IntType>>()
            .Description("Represents the command's unique identifier");
        descriptor.Field(c => c.CommandLine)
            .Type<NonNullType<StringType>>()
            .Description("Represents the command's description");

        descriptor.Field(c => c.PlatformId).Ignore();

        descriptor.Field(c => c.Platform)
            .ResolveWith<Resolvers>(r => r.GetPlatform(default!, default!))
            .Type<PlatformType>()
            .Description("The platform to which this command belongs");
    }

    private sealed class Resolvers
    {
        public Platform? GetPlatform([Parent] Command arg,
            [Service(ServiceKind.Synchronized)] DemoDbContext context)
        {
            return context.Platforms
                .FirstOrDefault(p => p != null && p.Id == arg.PlatformId);
        }
    }
}