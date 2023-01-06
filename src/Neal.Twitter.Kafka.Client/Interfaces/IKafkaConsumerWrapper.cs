using Confluent.Kafka;

namespace Neal.Twitter.Kafka.Client.Interfaces;

public interface IKafkaConsumerWrapper<TKey, TValue> : IDisposable
{
    Task<ConsumeResult<TKey, TValue>> ConsumeAsync(CancellationToken cancellationToken);
}