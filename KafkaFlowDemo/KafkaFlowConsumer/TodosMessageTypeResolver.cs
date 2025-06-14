using KafkaFlow;
using KafkaFlow.Middlewares.Serializer.Resolvers;
using Microsoft.Extensions.Logging;
using Models;

namespace KafkaFlowConsumer;

internal class TodosMessageTypeResolver(
    ILogger<TodosMessageTypeResolver> logger
) : IMessageTypeResolver
{
    public ValueTask<Type> OnConsumeAsync(IMessageContext context)
    {
        var messageTypeHeader = context.Headers.GetString(Constants.MessageHeader.Key);

        return messageTypeHeader switch
        {
            Constants.MessageHeader.WorkTodoValue => new ValueTask<Type>(typeof(WorkTodoEvent)),
            Constants.MessageHeader.TrainingTodoValue => new ValueTask<Type>(typeof(TrainingTodoEvent)),
            _ => throw new InvalidOperationException($"Unknown message type: {messageTypeHeader}")
        };
    }

    public ValueTask OnProduceAsync(IMessageContext context)
    {
        return ValueTask.CompletedTask;
    }
}