using GraphQL.Domain.Entities;
using GraphQL.Persistence.Context;
using HotChocolate.Subscriptions;

namespace GraphQL.Infra.GraphQL;

public class MyMutation
{
    public async Task<AddPlatformPayload> AddPlatformAsync(
        AddPlatformInput input,
        [Service(ServiceKind.Synchronized)] DemoDbContext context,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var platform = new Platform
        {
            Name = input.Name
        };

        await context.Platforms.AddAsync(platform, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await eventSender.SendAsync(nameof(Subscription.OnPlatformAdded), platform, cancellationToken);

        return new AddPlatformPayload(platform);
    }

    public async Task<AddCommandPayload> AddCommandAsync(AddCommandInput input,
        [Service(ServiceKind.Synchronized)] DemoDbContext context)
    {
        var command = new Command
        {
            Name = input.Name,
            CommandLine = input.CommandLine,
            PlatformId = input.PlatformId
        };

        await context.Commands.AddAsync(command);
        await context.SaveChangesAsync();

        return new AddCommandPayload(command);
    }
}

public record AddPlatformInput(string Name);

public record AddPlatformPayload(Platform Platform);

public record AddCommandPayload(Command Command);

public record AddCommandInput(string Name, string CommandLine, int PlatformId);