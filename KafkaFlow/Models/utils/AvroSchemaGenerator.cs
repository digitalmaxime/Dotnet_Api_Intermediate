using Chr.Avro.Abstract;
using Chr.Avro.Representation;

namespace Models.utils;

public static class AvroSchemaGenerator
{
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
}