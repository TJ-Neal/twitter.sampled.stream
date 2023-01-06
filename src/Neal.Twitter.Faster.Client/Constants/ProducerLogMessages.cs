namespace Neal.Twitter.Faster.Client.Constants;

public struct ProducerLogMessages
{
    public const string PrintResult = "{timestamp}: Faster producer: [status:{@status}] [{@id}].";

    public const string ProducerError = "Producer returned an error. Key: {@key}, Value: {@value}";

    public const string ProducerException = "Producer caused an exception: {ex}";
}