using KafkaFlow;

namespace KafkaFlowConsumer;

public class HelloMessageHandler : IMessageHandler<HelloMessage>
{
    public Task Handle(IMessageContext context, HelloMessage message)
    {
        Console.WriteLine(
            "Partition: {0} | Offset: {1} | Message: {2}",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            message.Text);

        return Task.CompletedTask;
    }
}

public class HelloMessage
{
    public string Text { get; set; } = default!;
}