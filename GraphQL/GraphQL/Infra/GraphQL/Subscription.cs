using GraphQL.Domain.Entities;

namespace GraphQL.Infra.GraphQL;

public class Subscription
{
    [Subscribe]
    [Topic]
    public Platform OnPlatformAdded([EventMessage] Platform platform) => platform;
}