namespace Notifier.Infrastructure.Kafka;

public class BackGroundKafkaConsumer<TKey, TValue> : BackgroundService
{
    private readonly KafkaConsumerConfig _config;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BackGroundKafkaConsumer(
        IOptions<KafkaConsumerConfig> config,
        IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _config = config.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Factory.StartNew(() =>
        ConsumeTopic(stoppingToken),
        stoppingToken,
        TaskCreationOptions.LongRunning,
        TaskScheduler.Current);

    private async Task ConsumeTopic(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var handler = scope.ServiceProvider.GetRequiredService<IKafkaHandler<TKey, TValue>>();

        var builder = new ConsumerBuilder<TKey, TValue>(_config).SetValueDeserializer(new KafkaDeserializer<TValue>());

        using IConsumer<TKey, TValue> consumer = builder.Build();

        consumer.Subscribe(_config.Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                if (result is null)
                    continue;

                await handler.HandleAsync(result.Message.Key, result.Message.Value);

                consumer.Commit(result);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }
    }
}
