using Confluent.SchemaRegistry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using SchemaRegistration.utils;


var builder = Host.CreateApplicationBuilder(args);

var schemaRegistryConfig = builder.Configuration.GetSection("SchemaRegistry");

builder.Services.Configure<SchemaRegistryConfig>(schemaRegistryConfig);

builder.Services.AddSingleton<ISchemaRegistryClient>(_ => new CachedSchemaRegistryClient(new Dictionary<string, string>
{
    { "schema.registry.url", "http://localhost:8081" }
}));

var host = builder.Build();

using var serviceScope = host.Services.CreateScope();
var serviceProvider = serviceScope.ServiceProvider;

// Generate schemas
var workTodoConfluentSchema = SchemaGenerator.GenerateSchema<WorkTodoEvent>();
var trainingTodoConfluentSchema = SchemaGenerator.GenerateSchema<TrainingTodoEvent>();

// Register schemas
using var schemaRegistryClient = serviceProvider.GetRequiredService<ISchemaRegistryClient>();
var workTodoSchemaId = await schemaRegistryClient.RegisterSchemaAsync("todos-Models.WorkTodoEvent", workTodoConfluentSchema);
var trainingSchemaId = await schemaRegistryClient.RegisterSchemaAsync("todos-Models.TrainingTodoEvent", trainingTodoConfluentSchema);

Console.WriteLine(workTodoSchemaId > 0
    ? $"Successfully registered schema 'todos-Models.WorkTodoEvent' with ID: {workTodoSchemaId}"
    : "Failed to register schema for 'todos-Models.WorkTodoEvent'.");

Console.WriteLine(trainingSchemaId > 0
    ? $"Successfully registered schema 'todos-Models.TrainingTodoEvent' with ID: {trainingSchemaId}"
    : "Failed to register schema for 'todos-Models.TrainingTodoEvent'.");