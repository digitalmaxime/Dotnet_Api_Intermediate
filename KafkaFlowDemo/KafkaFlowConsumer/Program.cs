using KafkaFlow;
using KafkaFlow.Serializer;
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
        .AddConsumer(consumer => consumer
            .Topic(Constants.TopicName)
            .WithGroupId("todo-consumer-group")
            .WithBufferSize(100)
            .WithWorkersCount(2)
            .WithAutoOffsetReset(AutoOffsetReset.Latest)
            .AddMiddlewares(middlewares => middlewares
                .AddDeserializer<JsonCoreDeserializer, TodosMessageTypeResolver>()
                .AddTypedHandlers(h => h
                    .AddHandler<WorkTodoMessageHandler>()
                    .AddHandler<TrainingMessageHandler>()
                    .WhenNoHandlerFound(context =>
                        Console.WriteLine("Message not handled > Partition: {0} | Offset: {1}",
                            context.ConsumerContext.Partition,
                            context.ConsumerContext.Offset)
                    )
                )
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