using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlowProducer.EndpointHandlers;
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

        group.MapPost("work", EndpointHandler.HandleWorkTodo);

        group.MapPost("training", EndpointHandler.HandleTrainingTodo);

        group.MapGet("", (TodoDbContext context) => Results.Ok((object?)context.Todos.ToList()));

        group.MapDelete("", (TodoDbContext context) =>
        {
            context.Todos.RemoveRange(context.Todos);
            return Results.NoContent();
        });
    }
}