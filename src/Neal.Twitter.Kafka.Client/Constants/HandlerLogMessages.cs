namespace Neal.Twitter.Kafka.Client.Constants;

public struct HandlerLogMessages
{
    public const string PrintResult = "{@timestamp}: Kafka producer status: {@status} on topic {@messageId}.";

    public const string ProducerException = "Producer returned an error. Key: {@key}, Value: {@value}, Topic: {@topic}, Error: {@error}";
}