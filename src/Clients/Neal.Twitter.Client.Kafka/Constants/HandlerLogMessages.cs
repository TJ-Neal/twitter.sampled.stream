namespace Neal.Twitter.Kafka.Client.Constants;

/// <summary>
/// Represents messages for logging handler events.
/// </summary>
public struct HandlerLogMessages
{
    public const string PrintProducerResult = "{timestamp}: Kafka status: {status} on topic {topic} [{count}].";

    public const string PrintConsumeResult = "{local_timestamp_utc}[UTC}: Kafka consumer processed message {id} with original timestamp of: {message_timestamp_utc}[UTC].";

    public const string ProducerException = "Producer returned an error. Key: {key}, Value: {value}, Topic: {topic}, Error: {error}";

    public const string ConsumerException = "Kafka consumer subscription returned an error. {message}";

    public const string ConsumerPostError = "Consumer post returned an error. Status: {status}, Content: {content}";
}