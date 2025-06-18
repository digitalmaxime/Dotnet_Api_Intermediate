using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaFlowProducer.Endpoints;
using KafkaFlowProducer.Persistence;
using KafkaFlow;

using Microsoft.Extensions.Options;
using Models;


var builder = WebApplication.CreateBuilder(args);

var schemaRegistryConfig = builder.Configuration.GetSection("SchemaRegistry");

builder.Services.Configure<SchemaRegistryConfig>(schemaRegistryConfig);

builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<SchemaRegistryConfig>>();
    return new CachedSchemaRegistryClient(config.Value);
});

builder.Services.AddKafka(kafka => kafka.AddCluster(cluster =>
{
    const string topicName = "todos";
    cluster
        .WithBrokers(["localhost:9092"])
        .WithSchemaRegistry(config => config.Url = "localhost:8081")
        .CreateTopicIfNotExists(topicName, 1, 1)
        .AddProducer("publish-todo-producer",
            producer => producer
                .DefaultTopic(topicName)
                .AddMiddlewares(middlewares =>
                    middlewares.AddSchemaRegistryAvroSerializer(new AvroSerializerConfig()
                    {
                        AutoRegisterSchemas = true,
                        NormalizeSchemas = true,
                        SubjectNameStrategy = SubjectNameStrategy.TopicRecord,
                    })
                )
        );
}));

builder.Services.AddSqlite<TodoDbContext>("Data Source=todos.db");

var app = builder.Build();

app.MapTodoEndpoints();

app.UseHttpsRedirection();

app.Run();
