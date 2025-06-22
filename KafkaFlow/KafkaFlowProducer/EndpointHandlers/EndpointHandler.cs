using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlowProducer.Entities;
using KafkaFlowProducer.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Models;

namespace KafkaFlowProducer.EndpointHandlers;

public static class EndpointHandler
{
    public static async Task<Results<Created<Todo>, InternalServerError<string>>> HandleTrainingTodo(Todo todo,
        IProducerAccessor producerAccessor, TodoDbContext context)
    {
        var headers = new MessageHeaders
        {
            {
                Constants.MessageHeader.Key,
                System.Text.Encoding.UTF8.GetBytes(Constants.MessageHeader.TrainingTodoValue)
            }
        };

        var messageValue = new TrainingTodoEvent()
        {
            Description = todo.Description
        };

        var producer = producerAccessor.GetProducer("publish-todo-producer");
        try
        {
            var deliveryReport =
                await producer.ProduceAsync(Constants.TopicName, todo.Title, messageValue, headers);

            if (deliveryReport.Status == PersistenceStatus.NotPersisted)
            {
                Console.WriteLine($"Training Message delivery failed: {@deliveryReport}");
                return TypedResults.InternalServerError(
                    $"Training Message delivery failed, the broker failed to persist the message.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Delivery failed (exception): {e.Message}");
            return TypedResults.InternalServerError(
                $"Training Message delivery failed: {e.Message}");
        }

        context.Todos.Add(todo);

        await context.SaveChangesAsync();

        return TypedResults.Created($"/api/todos/{todo.Id}", todo);
    }

    public static async Task<Results<Created<Todo>, InternalServerError<string>>> HandleWorkTodo(
        Todo todo, IProducerAccessor producerAccessor, TodoDbContext context)
    {
        var headers = new MessageHeaders
        {
            {
                Constants.MessageHeader.Key,
                System.Text.Encoding.UTF8.GetBytes(Constants.MessageHeader.WorkTodoValue)
            }
        };
        var messageValue = new WorkTodoEvent()
        {
            Description = todo.Description
        };

        var producer = producerAccessor.GetProducer("publish-todo-producer");
        var deliveryReport = await producer.ProduceAsync(Constants.TopicName, todo.Title, messageValue, headers);

        if (deliveryReport.Status == PersistenceStatus.NotPersisted)
        {
            Console.WriteLine($"Work Message delivery failed: {@deliveryReport}");
            return TypedResults.InternalServerError(
                $"Work Message delivery failed, the broker failed to persist the message.");
        }

        context.Todos.Add(todo);

        await context.SaveChangesAsync();

        return TypedResults.Created($"/api/todos/work/{todo.Id}", todo);
    }
}