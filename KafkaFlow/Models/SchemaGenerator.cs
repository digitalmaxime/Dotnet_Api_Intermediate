using Chr.Avro.Abstract;
using Chr.Avro.Representation;
using Confluent.SchemaRegistry;

namespace Models;

public static class SchemaGenerator
{
    public static Confluent.SchemaRegistry.Schema GenerateSchema<TEventType>()
    {
        // Generate schema string
        var builder = new SchemaBuilder();
        var schema = builder.BuildSchema<TEventType>();
        var jsonWriter = new JsonSchemaWriter();
        var fullSchemaJson = jsonWriter.Write(schema);
        
        // Write schema to file
        var projectDirectory = Directory.GetCurrentDirectory();
        var solutionDirectory = Directory.GetParent(projectDirectory)!.Parent!.Parent!.Parent!.FullName;
        var schemaPath = Path.Combine(solutionDirectory, "Models", "schemas", $"{typeof(TEventType).Name}.schema.json");
        File.WriteAllText(schemaPath, fullSchemaJson);
        
        // Convert to Confluent.SchemaRegistry.Schema
        var confluentSchema = new Confluent.SchemaRegistry.Schema(
            fullSchemaJson,
            SchemaType.Avro
        );

        return confluentSchema;
    }
    
    public static Avro.Schema GenerateAvroSchema<TEventType>()
    {
        var builder = new SchemaBuilder();
        var schema = builder.BuildSchema<TEventType>();
        var jsonWriter = new JsonSchemaWriter();
        var fullSchemaJson = jsonWriter.Write(schema);
        
        // Convert to Avro.Schema
        var avroSchema = Avro.Schema.Parse(fullSchemaJson);

        return avroSchema;
    }

    public static async Task<int> RegisterSchema(string subject, Confluent.SchemaRegistry.Schema schema)
    {
        // var schemaId = await schemaRegistryClient.RegisterSchemaAsync(subject, schema);

        // return schemaId;
        return 0;
    }
}
