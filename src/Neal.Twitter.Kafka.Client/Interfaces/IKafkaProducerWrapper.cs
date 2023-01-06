using Neal.Twitter.Application.Interfaces;
using Neal.Twitter.Core.Entities.Kafka;

namespace Neal.Twitter.Kafka.Client.Interfaces;

public interface IKafkaProducerWrapper : IProducerWrapper<KafkaProducerMessage>
{
    void Flush();
}