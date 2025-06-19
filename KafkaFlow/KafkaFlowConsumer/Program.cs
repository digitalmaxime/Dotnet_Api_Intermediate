using Confluent.SchemaRegistry;
using KafkaFlow;
using KafkaFlowConsumer.Handlers;
using Models;

var builder = WebApplication.CreateBuilder(args);

// var schemaRegistryConfig = builder.Configuration.GetSection("SchemaRegistry");
//
// builder.Services.Configure<SchemaRegistryConfig>(schemaRegistryConfig);

builder.Services.AddSingleton<ISchemaRegistryClient>(_ =>
    new CachedSchemaRegistryClient([new KeyValuePair<string, string>("Url", "http://localhost:8081")]));

var services = builder.Services;

services.AddKafka(kafka => kafka
    .UseConsoleLog()
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .WithSchemaRegistry(config => config.Url = "localhost:8081")
        .WithSchemaRegistry(schemaRegistry =>
        {
            schemaRegistry.ValueSubjectNameStrategy = SubjectNameStrategy.TopicRecord;
            schemaRegistry.Url = "http://localhost:8081";
        })
        .AddConsumer(
            consumer => consumer
                .Topic(Constants.TopicName)
                .WithGroupId("todo-consumer-group")
                .WithBufferSize(100)
                .WithWorkersCount(20)
                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                .AddMiddlewares(
                    middlewares =>
                    {
                        middlewares
                            .AddSchemaRegistryAvroDeserializer()
                            .AddTypedHandlers(
                                handlers => handlers
                                    .AddHandler<WorkTodoMessageHandler>()
                                // .AddHandler<TrainingMessageHandler>()
                            );
                    }
                )
        )
    )
);

var app = builder.Build();

var bus = app.Services.CreateKafkaBus();

await bus.StartAsync();

Console.WriteLine("press any key to exit");

Console.ReadKey();

await bus.StopAsync();