using Confluent.Kafka;

namespace Neal.Twitter.Application.Interfaces.Confluent.Kafka;

public interface IKafkaProducerWrapper
{
    Task Produce(string topic, Message<string, string> message, CancellationToken cancellationToken);

    void Flush();
}