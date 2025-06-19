using Confluent.SchemaRegistry;
using KafkaFlow;
using KafkaFlowConsumer;
using KafkaFlowConsumer.Handlers;
using Microsoft.Extensions.Options;
using Models;

var builder = WebApplication.CreateBuilder(args);

var kafkaConfigurations = builder.Configuration
    .GetSection(KafkaConfigurationOptions.SectionName)
    .Get<KafkaConfigurationOptions>();

builder.Services.AddSingleton<ISchemaRegistryClient>(_ =>
    new CachedSchemaRegistryClient(kafkaConfigurations!.SchemaRegistryConfig));

var services = builder.Services;

services.AddKafka(kafka => kafka
    .UseConsoleLog()
    .AddCluster(cluster => cluster
        .WithBrokers([kafkaConfigurations!.BootstrapServer])
        .WithSchemaRegistry(schemaRegistryConfiguration =>
        {
            schemaRegistryConfiguration.Url = kafkaConfigurations!.SchemaRegistryConfig.Url;
        })
        .AddConsumer(
            consumer => consumer
                .Topic(Constants.TopicName)
                .WithGroupId(kafkaConfigurations.ConsumerGroupId)
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
                                    .AddHandler<TrainingTodoMessageHandler>()
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