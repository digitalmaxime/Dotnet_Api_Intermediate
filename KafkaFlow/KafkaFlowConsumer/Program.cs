using KafkaFlow;
using KafkaFlowConsumer;
using Microsoft.Extensions.DependencyInjection;
using Models;

var services = new ServiceCollection();
services.AddSingleton<TodosMessageTypeResolver>();

services.AddLogging();
services.AddKafka(kafka => kafka
    .UseConsoleLog()
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .CreateTopicIfNotExists(Constants.TopicName, 1, 1)
        .AddConsumer(
            consumer => consumer
                .Topic(Constants.TopicName)
                .WithGroupId("todo-consumer-group")
                .WithBufferSize(100)
                .WithWorkersCount(20)
                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                .AddMiddlewares(
                    middlewares => middlewares
                        .AddSchemaRegistryAvroDeserializer()
                        .AddTypedHandlers(
                            handlers => handlers
                                .AddHandler<WorkTodoMessageHandler>()
                                .AddHandler<TrainingMessageHandler>())
                )
        )
    )
);

var serviceProvider = services.BuildServiceProvider();

var bus = serviceProvider.CreateKafkaBus();

await bus.StartAsync();

Console.WriteLine("press any key to exit");

Console.ReadKey();

await bus.StopAsync();