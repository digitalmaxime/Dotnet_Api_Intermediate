using System.Text.Json;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaFlowProducer.Endpoints;
using KafkaFlowProducer.Persistence;
using KafkaFlow;
using Models;
using KafkaFlow.Serializer.SchemaRegistry;


var builder = WebApplication.CreateBuilder(args);

var schemaRegistryConfig = builder.Configuration.GetSection("SchemaRegistry");

builder.Services.Configure<SchemaRegistryConfig>(schemaRegistryConfig);
var schemaRegistryClient =
    new CachedSchemaRegistryClient(schemaRegistryConfig.Get<Dictionary<string, string>>());

builder.Services.AddKafka(kafka => kafka.AddCluster(cluster =>
{
    const string topicName = "todos";
    cluster
        .WithBrokers(["localhost:9092"])
        .CreateTopicIfNotExists(topicName, 1, 1)
        .AddProducer("publish-todo-producer",
            producer => producer
                .DefaultTopic(topicName)
                .AddMiddlewares(middlewares =>
                    middlewares.AddSchemaRegistryAvroSerializer(new AvroSerializerConfig()
                    {
                        AutoRegisterSchemas = true,
                        SubjectNameStrategy = SubjectNameStrategy.TopicRecord,
                        NormalizeSchemas = true,
                    })
                )
        );
}));

builder.Services.AddOpenApi();
builder.Services.AddSqlite<TodoDbContext>("Data Source=todos.db");
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapTodoEndpoints();

app.UseHttpsRedirection();

app.Run();


public class MyDepRes : IDependencyResolver
{
    public IDependencyResolverScope CreateScope()
    {
        throw new NotImplementedException();
    }

    public object Resolve(Type type)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<object> ResolveAll(Type type)
    {
        throw new NotImplementedException();
    }
}