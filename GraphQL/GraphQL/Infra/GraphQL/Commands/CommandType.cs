using GraphQL.Domain.Entities;

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

        descriptor.Field(c => c.PlatformId);
        
        
    }
}