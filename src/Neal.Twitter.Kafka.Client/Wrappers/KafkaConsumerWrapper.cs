using Confluent.Kafka;
using Neal.Twitter.Kafka.Client.Interfaces;

namespace Neal.Twitter.Kafka.Client.Wrappers;

public class KafkaConsumerWrapper<TKey, TValue> : IKafkaConsumerWrapper<TKey, TValue>
{
    private readonly IConsumer<TKey, TValue> consumer;

    public KafkaConsumerWrapper(string bootstrapServers, string groupId, AutoOffsetReset autoOffsetReset)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = autoOffsetReset
        };

        this.consumer = new ConsumerBuilder<TKey, TValue>(config).Build();
    }

    public async Task<ConsumeResult<TKey, TValue>> ConsumeAsync(CancellationToken cancellationToken) => await Task.Run(() => this.consumer.Consume(cancellationToken));

    public void Dispose()
    {
        this.consumer.Close();
        GC.SuppressFinalize(this);
    }
}