using System.Collections.Immutable;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Models;
using SchemaRegistration;


// var builder = Host.CreateDefaultBuilder(args);
var builder = Host.CreateApplicationBuilder(args);
var schemaRegistryConfig = builder.Configuration.GetSection("SchemaRegistry");
builder.Services.Configure<SchemaRegistryConfig>(schemaRegistryConfig);

builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<SchemaRegistryConfig>>();
    // return new CachedSchemaRegistryClient(config.Value);
    return new CachedSchemaRegistryClient(new Dictionary<string, string> 
    { 
        { "schema.registry.url", "http://localhost:8081" }
    });
});

var host = builder.Build();

using IServiceScope seviceProvider = host.Services.CreateScope();
var serviceProvider = seviceProvider.ServiceProvider;

using var schemaRegistryClient = serviceProvider.GetRequiredService<ISchemaRegistryClient>();
var avroSchemaGenerator = new AvroSchemaGenerator(schemaRegistryClient);
var confluentSchema = avroSchemaGenerator.GenerateSchema<WorkTodoEvent>();

var schemaId = await avroSchemaGenerator.RegisterSchema("todos-Models.WorkTodoEvent", confluentSchema);