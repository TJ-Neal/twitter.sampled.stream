namespace Neal.Twitter.Kafka.Client.Interfaces;

public interface IKafkaConsumerWrapper<TKey, TValue> : IDisposable
{
    Task ConsumeAsync(CancellationToken cancellationToken);
}