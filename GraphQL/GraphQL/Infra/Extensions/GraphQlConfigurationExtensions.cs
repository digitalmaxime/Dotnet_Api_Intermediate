using GraphQL.Infra.GraphQL.Platforms;
using GraphQL.Infra.GraphQL.Queries;
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
            .AddType<PlatformType>()
            ;
    }
}