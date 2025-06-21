using Chr.Avro.Abstract;
using Chr.Avro.Representation;
using Confluent.SchemaRegistry;
// Add this using directive

// Add this using directive

namespace SchemaRegistration.utils;

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
        var solutionDirectory = FindSolutionDirectory() 
            ?? throw new DirectoryNotFoundException("Could not find the solution directory. The search upwards from the executable's location failed.");
            
        var schemaPath = Path.Combine(solutionDirectory, "Models", "schemas", $"{typeof(TEventType).Name}.schema.json");
        
        // Ensure the directory exists before writing the file
        Directory.CreateDirectory(Path.GetDirectoryName(schemaPath)!);
        File.WriteAllText(schemaPath, fullSchemaJson);

        // Convert to Confluent.SchemaRegistry.Schema
        var confluentSchema = new Confluent.SchemaRegistry.Schema(
            fullSchemaJson,
            SchemaType.Avro
        );

        return confluentSchema;
    }

    /// <summary>
    /// Finds the solution directory by searching upwards from the application's base directory
    /// for a file with the .sln extension.
    /// </summary>
    /// <returns>The full path to the solution directory, or null if not found.</returns>
    private static string? FindSolutionDirectory()
    {
        // AppContext.BaseDirectory is more reliable than Directory.GetCurrentDirectory()
        var currentDirectory = AppContext.BaseDirectory;

        while (currentDirectory != null)
        {
            // Look for a .sln file in the current directory.
            if (Directory.EnumerateFiles(currentDirectory, "*.sln").Any())
            {
                return currentDirectory;
            }

            // Move up to the parent directory.
            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        return null; // Solution directory not found.
    }
}