using KafkaFlow;

namespace KafkaFlowConsumer;

public class KafkaBusService(
    ILogger<KafkaBusService> logger,
    IServiceProvider serviceProvider) : IHostedService
{
    private readonly IKafkaBus _bus = serviceProvider.CreateKafkaBus();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("✅ Kafka consumer service is starting...");
        await _bus.StartAsync(cancellationToken);
        logger.LogInformation("✅ Consumer is now running and listening for messages.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("🛑 Kafka consumer service is stopping...");
        await _bus.StopAsync();
        logger.LogInformation("🛑 Service has been shut down.");
    }
}