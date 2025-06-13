using KafkaFlowDemo.Endpoints;
using KafkaFlowDemo.Persistence;
using KafkaFlow;
using KafkaFlow.Serializer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafka(kafka => kafka.AddCluster(cluster =>
{
    const string topicName = "todos";
    cluster
        .WithBrokers(new[] { "localhost:9092" })
        .CreateTopicIfNotExists(topicName, 1, 1)
        .AddProducer("publish-todo-producer",
            producer => producer
                .DefaultTopic(topicName)
                .AddMiddlewares(middlewares =>
                    // middlewares.AddSerializer<CustomJsonSerializer>()
                    middlewares.AddSerializer<JsonCoreSerializer>()
                )
        );
}));

builder.Services.AddOpenApi();
builder.Services.AddSqlite<TodoDbContext>("Data Source=todos.db");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapTodoEndpoints();

app.UseHttpsRedirection();

app.Run();