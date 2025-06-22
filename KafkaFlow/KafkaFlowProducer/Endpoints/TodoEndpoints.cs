using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlowProducer.Entities;
using KafkaFlowProducer.Persistence;
using Models;

namespace KafkaFlowProducer.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api/todos")
            .WithDescription("A group of endpoints to manage todos")
            .WithTags("todos");

        group.MapPost("work", async (Todo todo, IProducerAccessor producerAccessor, TodoDbContext context) =>
        {
            var producer = producerAccessor.GetProducer("publish-todo-producer");
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
            
            await producer.ProduceAsync(Constants.TopicName, "messageKey", new WorkTodoEvent()
            {
                Description = "avro message"
            }, headers);
            
            context.Todos.Add(todo);
            
            await context.SaveChangesAsync();
            
            return Results.Created($"/api/todos/{todo.Id}", todo);
        });
        
        group.MapPost("training", async (Todo todo, IProducerAccessor producerAccessor, TodoDbContext context) =>
        {
            var producer = producerAccessor.GetProducer("publish-todo-producer");
            var headers = new MessageHeaders
            {
                {
                    Constants.MessageHeader.Key,
                    System.Text.Encoding.UTF8.GetBytes(Constants.MessageHeader.TrainingTodoValue)
                }
            };
            
            context.Todos.Add(todo);
            
            await context.SaveChangesAsync();
            
            var messageValue = new TrainingTodoEvent()
            {
                Description = todo.Description
            };
            var toto = await producer.ProduceAsync(Constants.TopicName, "messageKey", messageValue, headers);
            
            return Results.Created($"/api/todos/{todo.Id}", todo);
        });

        group.MapGet("", (TodoDbContext context) => { return Results.Ok(context.Todos.ToList()); });

        group.MapDelete("", (TodoDbContext context) =>
        {
            context.Todos.RemoveRange(context.Todos);
            return Results.NoContent();
        });
    }
}