namespace Notifier.Infrastructure.Kafka;

public interface IKafkaHandler<in TKey, in TValue>
{
    Task HandleAsync(TKey key, TValue value);
}
