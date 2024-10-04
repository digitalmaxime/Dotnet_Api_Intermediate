using GraphQL.Infra.GraphQL.Queries;
using GraphQL.Persistence.Context;

namespace GraphQL.Infra.GraphQL.Extensions;

public static class GraphQlConfigurationExtensions
{
    public static void RegisterGraphQlServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddGraphQLServer()
            .RegisterDbContext<DemoDbContext>()
            .AddQueryType<Query>();
    }
}