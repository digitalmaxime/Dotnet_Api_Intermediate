using GraphQL.Infra.GraphQL;
using GraphQL.Infra.GraphQL.Commands;
using GraphQL.Infra.GraphQL.Platforms;
using GraphQL.Persistence.Context;

namespace GraphQL.Infra.Extensions;

public static class GraphQlConfigurationExtensions
{
    public static void RegisterGraphQlServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddGraphQLServer()
            .RegisterDbContext<DemoDbContext>()
            .AddQueryType<MyQuery>()
            .AddType<QueryType>()
            .AddType<PlatformType>()
            .AddType<CommandType>()
            .AddFiltering()
            .AddMutationType<MyMutation>()
            .AddSubscriptionType<Subscription>()
            .AddInMemorySubscriptions()
            ;
    }
}