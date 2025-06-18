using Chr.Avro.Abstract;
using Chr.Avro.Representation;
using Confluent.SchemaRegistry;

namespace SchemaRegistration;

public class AvroSchemaGenerator(ISchemaRegistryClient schemaRegistryClient)
{
    public Confluent.SchemaRegistry.Schema GenerateSchema<TEventType>()
    {
        // Generate schema string
        var builder = new SchemaBuilder();
        var schema = builder.BuildSchema<TEventType>();
        var jsonWriter = new JsonSchemaWriter();
        var fullSchemaJson = jsonWriter.Write(schema);
        
        // Write schema to file
        var projectDirectory = Directory.GetCurrentDirectory();
        var solutionDirectory = Directory.GetParent(projectDirectory)!.Parent!.Parent!.FullName;
        var schemaPath = Path.Combine(solutionDirectory,"schemas", $"{typeof(TEventType).Name}.schema.json");
        File.WriteAllText(schemaPath, fullSchemaJson);
        
        // Convert to Confluent.SchemaRegistry.Schema
        var confluentSchema = new Confluent.SchemaRegistry.Schema(
            fullSchemaJson,
            SchemaType.Avro
        );

        return confluentSchema;
    }

    public async Task<int> RegisterSchema(string subject, Confluent.SchemaRegistry.Schema schema)
    {
        var schemaId = await schemaRegistryClient.RegisterSchemaAsync(subject, schema);

        return schemaId;
    }

    // Simple schema generation example
    public void SimpleSchemaGeneration()
    {
        // var generator = new AvroSchemaGenerator();
        //
        // // Generate schema
        // var schema = generator.GenerateSchema<WorkTodoEvent>();
        //
        // // Convert to string representation
        // string schemaJson = schema.ToString();
        // Console.WriteLine(schemaJson);
        //
        // // Convert to Avro.Schema if needed
        // Avro.Schema avroSchema = generator.ConvertToAvroSchema(schema);
    }
}