using System.Runtime.Serialization;
using Avro;
using Avro.Specific;

namespace Models;

public class TrainingTodoEvent : ISpecificRecord
{
    public required string Description { get; set; } = "default description";

    public object Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => Description ?? throw new InvalidOperationException("Cannot get null field"),
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
            default:
                throw new IndexOutOfRangeException($"Cannot put unknown field with pos {fieldPos}");
        }
    }

    [IgnoreDataMember]
    public Schema Schema => SchemaGenerator.GenerateAvroSchema<TrainingTodoEvent>();
}