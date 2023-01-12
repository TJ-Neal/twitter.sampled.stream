namespace Neal.Twitter.Client.Kafka.Interfaces;

/// <summary>
/// Represents a wrapper for consuming log messages from the Kafka consumer.
/// </summary>
public interface IKafkaConsumerWrapper<TKey, TValue> : IDisposable
{
    Task ConsumeAsync(CancellationToken cancellationToken);
}