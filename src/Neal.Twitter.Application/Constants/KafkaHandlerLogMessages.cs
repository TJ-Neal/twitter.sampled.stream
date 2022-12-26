namespace Neal.Twitter.Application.Constants;

public struct KafkaHandlerLogMessages
{
    #region Fields

    public const string PrintResult = "{timestamp}: Kafka producer status: {status} on topic {messageId}.";

    public const string ProducerException = "Producer returned an error. Key: {key}, Value: {@value}, Topic: {@topic}";

    #endregion Fields
}