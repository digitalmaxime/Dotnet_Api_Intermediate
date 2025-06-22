using KafkaFlow;
using Models;

namespace KafkaFlowConsumer.Handlers;

public class WorkTodoMessageHandler : IMessageHandler<WorkTodoEvent>
{
    public Task Handle(IMessageContext context, WorkTodoEvent message)
    {
        Console.WriteLine(
            "Work Todo Event!\n" +
            "Partition: {0} | Offset: {1} | Message: {2}",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            message.Description
            );

        return Task.CompletedTask;
    }
}
