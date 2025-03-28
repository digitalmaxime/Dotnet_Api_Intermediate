using System.Text.Json;
using KafkaFlow;

namespace KafkaFlowDemo;

public class CustomJsonSerializer : ISerializer
{
    public Task SerializeAsync(object message, Stream output, ISerializerContext context)
    {
        return JsonSerializer.SerializeAsync(output, message);
    }
}