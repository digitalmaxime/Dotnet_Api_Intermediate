# GraphQL with .NET Core

Graphql demo with .net core and Hotchocolate using dockerized database

Visualizing the data with GraphQL localhost:5000/graphql

## Implementation steps

- [Ingredients and Tooling](#ingredients-and-tooling)
- [GraphQL Theory](#graphql-theory)
- [Project Setup](#project-setup)
- [Introducing Types](#introducing-types)
- [Filtering and Sorting](#filtering-and-sorting)
- [Mutations](#mutations)
- [Subscriptions](#subscriptions)
- [Consuming GraphQL](#consuming-graphql)
- [Documentation](#documentation)
- [Reference](#reference)

## Ingredients and Tooling

- `HotChocolate.AspNetCore`
- `HotChocolate.Data.EntityFramework`
- `GraphQL.Server.Ui.Voyager` for the UI of GraphQL (non essential)
- `Microsoft.EntityFrameworkCore.Design` for dotnet-ef migrations
- `Microsoft.EntityFrameworkCore.SqlServer` for SQL Server

## GraphQL Theory

### What is GraphQL?

GraphQL is a query and manipulation language for APIs

GraphQL is also the runtime for fulfilling request

Created at facebook in 2012 to address both over and under fetching

Open Source in 2015 hosted by the Linux Foundation

### Why GraphQL?

**Rest** does _and_ under overfetching..

**GraphQL** allows you to request only the data you need

### Core Concepts

- **Schema**
    - describes the API in full
    - Self documenting
    - Comprised of "Types"
    - Must have a "Root Query Type"
        ```
        schema {
            query: MyQuery
        }
        ```
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
        ```
    - Query
        - Entry point for the API
      ```
      schema {
          query: MyQuery
      }
      
      type MyQuery {
         platform: [Platform!]!
         commands: [Command!]!
      }
      ```
    - Mutation
        - Entry point for changing data
    - Subscription
        - Entry point for real-time data

- **Resolver**
    - A resolver returns data for a given field
    - They can resolve to _anything_
    - They are defined within the ObjectType<> class
        - e.g. `Field(p => p.Id).ResolveWith<Resolvers>(r => r.GetId(default, default));`

- **Data Source**
    - Where the data comes from
    - Could be a database, REST API, etc


## Project Setup

### Main Configs and Startup

```csharp
serviceCollection
            .AddGraphQLServer()
            .AddQueryType<Query>()
```

```csharp
app.MapGraphQL();
app.UseGraphQLVoyage(new GraphQLVoyagerOptions()); // usefull for visualizing the schema
```

### Revisit DbContext

When using "alias" in the query, the query is executed multiple times

e.g. in postman or insomnia using aliases 'a', 'b', 'c'

```graphql
query {
    a: platform {
        name
        description
    }
    b: platform {
        name
        commands {
            commandLine
        }
    }
    c: platform {
        id
        description
    }
}
```

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


## Introducing Types

We want to decouple the application's entities / models from the GraphQL types

In order to achieve this, we can create a new class for each entity

The `PlatformType` class will inherit from hotchocolate's `ObjectType<Platform>` and override the `Configure` method

```csharp
public class PlatformType : ObjectType<Platform>
{
    protected override void Configure(IObjectTypeDescriptor<Platform> descriptor)
    {
        descriptor
            .Field(p => p.Id)
            .Type<NonNullType<IntType>>();

        descriptor
            .Field(p => p.LiscenceKey)
            .Ignore(); <--- Ignore this field
```

This way, GraphQL will expose only the fields we want.

But we need to add the `PlatformType` to the extension method `AddType<PlatformType>()` in the `ServiceCollection`

```csharp
serviceCollection
            .AddGraphQLServer()
            .RegisterDbContext<DemoDbContext>()
            .AddQueryType<MyQuery>()
            .AddType<PlatformType>() <--- Add this line
```


## Filtering and Sorting

```csharp
services.AddGraphQLServer()
    // Your schema configuration
    .AddFiltering();
```

```csharp
public class QueryType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor
            .Field(f => f.GetUsers(default))
            .Type<ListType<NonNullType<UserType>>>()
            .UseFiltering(); <--- Add this line
    }
}
```

Don't forget to add the QueryType in the `ServiceCollection`

```csharp
services.AddGraphQLServer()
    // Your schema configuration
    .AddQueryType<QueryType>()
    .AddFiltering(); <--- Add this line
```

query example : 
```graphql
query Platforms {
    platforms(where: {id : {eq: 2}}) {
        id
        name
        description
    }
}
```

## Mutations

Allows for data update

Is homologous to Query Type, but while query fields are executed in parallel, mutation fields run in series, one after the other.

A mutation type can take an Input and return a Payload which are kinda like data transfer object.

C# example code 
```csharp
public async Task<AddPlatformInput> AddPlatformAsync(AddPlatformInput input, [Service(ServiceKind.Synchronized)] DemoDbContext) {
    var platform = new Platform { ... }
    // db context Add + Save
    return new AddPlatformPayload(platform);
}

```

Query example code
```graphql
mutation AddPlatform {
    addPlatform(input: {name: "Azure"}) {
        platform {
            id
            name
            description
        }
    }
}
```


## Subscriptions

works using websockets

```csharp
// program.cs
app.UseWebSockets(); <--- Add this line to the request pipeline
```

```csharp
services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddFiltering()
    .AddSorting()
    .AddSubscriptionType<Subscription>() <--- Add this line
    .AddInMemorySubscriptions(); <--- Add this line
```


## Consuming GraphQL

https://localhost:7156/graphql/

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

## Documentation

We can add Description to the fields and types

Using GraphQL Voyager to visualize the schema (with the `GraphQL.Server.Ui.Voyager` package)

https://localhost:7156/graphql-voyager

## Reference
- [GraphQL](https://graphql.org/)
- [Les Jackson](https://www.youtube.com/watch?v=HuN94qNwQmM&ab_channel=LesJackson)
- [HotChocolate](https://chillicream.com/docs/hotchocolate/getting-started)
- [HotChocolate with EntityFrameWork](https://chillicream.com/docs/hotchocolate/v13/integrations/entity-framework)
- [HotChocolate Resolvers](https://chillicream.com/docs/hotchocolate/v12/fetching-data/resolvers)