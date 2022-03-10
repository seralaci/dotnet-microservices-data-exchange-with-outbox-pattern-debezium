namespace Notifier.Infrastructure.Kafka;

public static class RegisterServiceExtensions
{
    public static IServiceCollection AddKafkaConsumer<TKey, TValue, THandler>(
        this IServiceCollection services,
        Action<KafkaConsumerConfig> configAction) where THandler : class, IKafkaHandler<TKey, TValue>
    {
        services.AddScoped<IKafkaHandler<TKey, TValue>, THandler>();

        services.AddHostedService<BackGroundKafkaConsumer<TKey, TValue>>();

        services.Configure(configAction);

        return services;
    }
}