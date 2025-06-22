using Confluent.SchemaRegistry;
using KafkaFlow;
using KafkaFlowConsumer;
using KafkaFlowConsumer.Handlers;
using Models;

var builder = Host.CreateApplicationBuilder(args);

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

builder.Services.AddHostedService<KafkaBusService>();

var app = builder.Build();

await app.RunAsync();