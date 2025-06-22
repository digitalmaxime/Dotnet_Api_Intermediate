using Confluent.SchemaRegistry;

namespace KafkaFlowConsumer;

public class KafkaConfigurationOptions
{
    public const string SectionName = "Kafka";
    public required string TopicName { get; set; }
    public required SchemaRegistryConfig SchemaRegistryConfig { get; set; }
    public required string BootstrapServer { get; set; }
    public required string ConsumerGroupId { get; set; }
}