using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Avro;
using Avro.Specific;


namespace Models;

public class WorkTodoEvent : ISpecificRecord

{
    [Required] public required string Description { get; set; }

    public string? Note { get; set; }

    [DefaultValue("")] public string? Note2 { get; set; }

    [DefaultValue("")] public string? Note3 { get; set; }

    public object Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => Description ?? throw new InvalidOperationException("Cannot get null field"),
            1 => Note ?? string.Empty,
            2 => Note2 ?? string.Empty,
            3 => Note3 ?? string.Empty,
            _ => throw new IndexOutOfRangeException($"Cannot get unknown field with pos {fieldPos}")
        } ?? throw new InvalidOperationException("Cannot get null field :)");
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                Description = (string)fieldValue;
                break;
            case 1:
                Note = (string)fieldValue;
                break;
            case 2:
                Note2 = (string)fieldValue;
                break;
            case 3:
                Note3 = (string)fieldValue;
                break;
            default:
                throw new IndexOutOfRangeException($"Cannot put unknown field with pos {fieldPos}");
        }
    }


    [IgnoreDataMember] // This tells Avro to ignore this property when generating schema
    public Schema Schema => Schema.Parse(@"{
        ""type"": ""record"",
        ""name"": ""WorkTodoEvent"",
        ""namespace"": ""Models"",
        ""fields"": [
            { ""name"": ""Description"", ""type"": ""string"" },
            { ""name"": ""Note"", ""type"": [""null"", ""string""], ""default"": null },
            { ""name"": ""Note2"", ""type"": [""null"", ""string""], ""default"": """" },
            { ""name"": ""Note3"", ""type"": [""null"", ""string""], ""default"": """" }
        ]
    }");
}