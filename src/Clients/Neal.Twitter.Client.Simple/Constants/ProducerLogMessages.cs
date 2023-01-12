namespace Neal.Twitter.Simple.Client.Constants;

/// <summary>
/// Represents messages for logging producer messages.
/// </summary>
public struct ProducerLogMessages
{
    public const string Success = "{timestamp}: Tweet Repository producer status: {status} for message {messageId}.";

    public const string ProducerError = "Producer returned an error. Key: {key}, Value: {value}";

    public const string ProducerException = "Producer caused an exception: {ex}";
}