using System.Text.Json;
using KafkaFlow;
using KafkaFlow.Middlewares.Serializer.Resolvers;
using KafkaFlow.Serializer;
using KafkaFlowConsumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using static System.Text.Json.JsonSerializer;

var services = new ServiceCollection();
services.AddSingleton<MyMessageTypeResolver>(); // Or AddTransient or AddScoped, depending on your needs

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
                .AddDeserializer<JsonCoreDeserializer, MyMessageTypeResolver>()
                .AddTypedHandlers(h => h
                    .AddHandler<HelloMessageHandler>()
                    .AddHandler<GoodByeMessageHandler>()
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


internal class MyMessageTypeResolver(
    ILogger<MyMessageTypeResolver> logger
) : IMessageTypeResolver
{
    public ValueTask<Type> OnConsumeAsync(IMessageContext context)
    {
        var messageTypeHeader = context.Headers.GetString("message-type");

        return messageTypeHeader switch
        {
            MessageTypes.HelloMessage => new ValueTask<Type>(typeof(HelloMessage)),
            MessageTypes.GoodbyeMessage => new ValueTask<Type>(typeof(GoodByeMessage)),
            _ => throw new InvalidOperationException($"Unknown message type: {messageTypeHeader}")
        };

    }

    public ValueTask OnProduceAsync(IMessageContext context)
    {
        return ValueTask.CompletedTask;
    }
}

public static class MessageTypes
{
    public const string HelloMessage = "hello-message";
    public const string GoodbyeMessage = "goodbye-message";
}
