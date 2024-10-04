# GraphQL with .NET Core

Demo using dockerized mssql and graphql with .net core

Visualizing the data with GraphQL localhost:5000/graphql

## Implementation steps

- [Ingredients and Tooling](#ingredients-and-tooling)
- [GraphQL Theory](#graphql-theory)
- [Project Setup](#project-setup)
- [Multi-Model](#multi-model)
- [Annotation vs Code First](#annotation-vs-code-first)
- [Introducing Types](#introducing-types)
- [Filtering and Sorting](#filtering-and-sorting)
- [Mutations](#mutations)
- [Subscriptions](#subscriptions)
- [Consuming GraphQL](#Consuming-GraphQL)
- [Reference](#reference)

## Ingredients and Tooling

- `HotChocolate.AspNetCore`
- `HotChocolate.Data.EntityFramework`
- `GraphQL.Server.Ui.Voyager` for the UI of GraphQL
- `Microsoft.EntityFrameworkCore.Design` for dotnet-ef migrations
- `Microsoft.EntityFrameworkCore.SqlServer` for SQL Server

## GraphQL Theory

### What is GraphQL?

GraphQL is a query and manipulation language for APIs

GraphQL is also the runtime for fulfilling request

Created at facebook in 2012 to address both over and under fetching

Open Source in 2015 hosted by the Linux Foundation

### Why GraphQL?

**Rest** does overfetching

**GraphQL** allows you to request only the data you need

**Rest** does underfetching

### Core Concepts

- **Schema**
    - describes the API in full
    - Self documenting
    - Comprised to "Types"
    - Must have a "Root Query Type"
- **Types**
    - Scalar
        - Int, Float, String, Boolean, ID
    - Object
        - Custom types
         ```
            type: Person {
                id: ID!
                name: String!
                age: Int
            }
    - Query
        - Entry point for the API
    - Mutation
        - Entry point for changing data
    - Subscription
        - Entry point for real-time data

- **Resolver**
    - A resolver returns data for a given field
    - They can resolve to _anything_

- **Data Source**
    - Where the data comes from
    - Could be a database, REST API, etc

### GraphQL .NET

## Project Setup

### Main Configs and Startup

```csharp
serviceCollection
            .AddGraphQLServer()
            .AddQueryType<Query>()
```

```csharp
app.MapGraphQL();
app.UseGraphQLVoyage(new GraphQLVoyagerOptions());
```

### Revisit DbContext

When using "alias" in the query, the query is executed multiple times

GraphQL will execute the query for each alias, but the DbContext fails to handle all three queries at the same time

We need to enable some form of multi-threading in the DbContext or force the DbContext to be access in a thread-safe way

```
serviceCollection
            .AddGraphQLServer()
            .RegisterDbContext<DemoDbContext>() <--- Add this line
            .AddQueryType<Query>();
```

In the Query class, we need to add the DbContext as a parameter and decorate it with the "Syncronized" attribute

```csharp
public IQueryable<Platform> GetPlatform([Service(ServiceKind.Synchronized)] DemoDbContext context)
    {
        return context.Platforms;
    }
```

Another option is to use the "AddPooledDbContextFactory", which is more complex but more efficient 

see [pooled dbcontext with hotchocolat](https://chillicream.com/docs/hotchocolate/v13/integrations/entity-framework#dbcontextkindpooled)


## Multi-Model

## Annotation vs Code First

## Introducing Types

## Filtering and Sorting

## Mutations

## Subscriptions

## Consuming GraphQL

```graphql
query {
  people {
    id
    name
    age
  }
}
```

using an "alias" (allPeople, youngPeople, olderPeople)
this execute multiple queries in one request

```graphql
query {
  allPeople: people {
    id
    name
    age
  }
  youngPeople: people(where: {age: {lt: 30}}) {
    id
    name
    age
  }
  olderPeople: people(where: {age: {gte: 30}}) {
    id
    name
    age
  }
 }
```

## Reference

- [Les Jackson](https://www.youtube.com/watch?v=HuN94qNwQmM&ab_channel=LesJackson)
- [HotChocolate](https://chillicream.com/docs/hotchocolate/getting-started)
- [HotChocolate with EntityFrameWork](https://chillicream.com/docs/hotchocolate/v13/integrations/entity-framework)