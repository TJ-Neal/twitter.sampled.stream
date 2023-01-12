using Neal.Twitter.Application.Interfaces;
using Neal.Twitter.Core.Entities.Kafka;

namespace Neal.Twitter.Client.Kafka.Interfaces;

/// <summary>
/// Represents a wrapper for events produced for the Kafka producer wrapper.
/// </summary>
public interface IKafkaProducerWrapper : IProducerWrapper<KafkaProducerMessage>
{
    void Flush();
}