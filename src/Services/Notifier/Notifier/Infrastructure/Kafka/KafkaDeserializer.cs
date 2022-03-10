using System.Text;
using System.Text.Json;

namespace Notifier.Infrastructure.Kafka;

internal sealed class KafkaDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var dataJsonString = Encoding.UTF8.GetString(data);

        // deserializing twice because of double serialization of event payload.
        var normalizedJsonString = JsonSerializer.Deserialize<string>(dataJsonString);

        return JsonSerializer.Deserialize<T>(normalizedJsonString);
    }
}