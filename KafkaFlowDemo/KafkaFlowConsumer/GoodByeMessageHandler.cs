using KafkaFlow;

namespace KafkaFlowConsumer;

public class GoodByeMessageHandler : IMessageHandler<GoodByeMessage>
{
    public Task Handle(IMessageContext context, GoodByeMessage message)
    {
        Console.WriteLine(
            "Good bye!\n" +
            "Partition: {0} | Offset: {1} | Message: {2}",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            message.Text);

        return Task.CompletedTask;
    }
}

public class GoodByeMessage
{
    public string Text { get; set; } = default!;
}