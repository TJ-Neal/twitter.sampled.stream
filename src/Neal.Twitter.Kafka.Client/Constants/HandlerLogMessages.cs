namespace Neal.Twitter.Kafka.Client.Constants;

public struct HandlerLogMessages
{
    public const string PrintProducerResult = "{timestamp}: Kafka status: {status} on topic {topic}.";

    public const string PrintConsumerResult = "{local_timestamp_utc}[UTC}: Kafka consumer processed message {id} with original timestamp of: {message_timestamp_utc}[UTC].";

    public const string ProducerException = "Producer returned an error. Key: {key}, Value: {value}, Topic: {topic}, Error: {error}";
}