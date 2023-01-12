namespace Neal.Twitter.Client.Kafka.Constants;

public struct WrapperLogMessages
{
    public const string ProducerFlush = "Kafka producer is being flushed.";

    public const string ProducerDispose = "KafkaProducerWrapper is being disposed.";

    public const string Subscribed = "Subscribed to [topic] {topic}";
}