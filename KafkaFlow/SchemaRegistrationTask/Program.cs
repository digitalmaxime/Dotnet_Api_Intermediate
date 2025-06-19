using Confluent.SchemaRegistry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Models;


var builder = Host.CreateApplicationBuilder(args);

var schemaRegistryConfig = builder.Configuration.GetSection("SchemaRegistry");

builder.Services.Configure<SchemaRegistryConfig>(schemaRegistryConfig);

builder.Services.AddSingleton<ISchemaRegistryClient>(_ => new CachedSchemaRegistryClient(new Dictionary<string, string>
{
    { "schema.registry.url", "http://localhost:8081" }
}));

var host = builder.Build();

using IServiceScope seviceProvider = host.Services.CreateScope();
var serviceProvider = seviceProvider.ServiceProvider;
using var schemaRegistryClient = serviceProvider.GetRequiredService<ISchemaRegistryClient>();
var workTodoConfluentSchema = SchemaGenerator.GenerateSchema<WorkTodoEvent>();
var trainingTodoConfluentSchema = SchemaGenerator.GenerateSchema<TrainingTodoEvent>();

var schemaId = await SchemaGenerator.RegisterSchema("todos-Models.WorkTodoEvent", workTodoConfluentSchema);