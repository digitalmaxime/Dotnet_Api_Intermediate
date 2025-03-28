using Confluent.Kafka;
using KafkaFlow.Producers;
using KafkaFlowDemo.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace KafkaFlowDemo.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api/todos")
            .WithDescription("A group of endpoints to manage todos")
            .WithTags("todos");

        group.MapPost("", async (Todo todo, IProducerAccessor producerAccessor, TodoDbContext context) =>
        {
            context.Todos.Add(todo);
            await context.SaveChangesAsync();
            var producer = producerAccessor.GetProducer("publish-todo-producer");
            // var headers = new Headers
            // {
            //     new Header("headerKey", []),
            // };
            // var message = new Message<string, Todo>()
            // {
            //     Headers = headers,
            //     Key = "messageKey",
            //     Value = todo,
            //     Timestamp = new Timestamp(DateTimeOffset.UtcNow)
            // };
            await producer.ProduceAsync("key", todo);
            return Results.Created($"/api/todos/{todo.Id}", todo);
        });
        
        group.MapGet("", (TodoDbContext context) =>
        {
            return Results.Ok(context.Todos.ToList());
        });

        group.MapDelete("", (TodoDbContext context) =>
        {
            context.Todos.RemoveRange(context.Todos);
            return Results.NoContent();
        });
    }
    
}