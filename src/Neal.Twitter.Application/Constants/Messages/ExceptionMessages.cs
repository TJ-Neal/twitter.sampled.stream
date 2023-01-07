namespace Neal.Twitter.Application.Constants.Messages;

public struct ExceptionMessages
{
    public const string ErrorDuringLoop = "Failed during loop executions\n{exception}";

    public const string KeyNotFound = "Unable to read key.";

    public const string SerializationError = "{name} is not valid cannot be serialized.";

    public const string GenericException = "Exception encountered during execution: {message}";

    public const string DisposeException = "There was an exception while disposing [{className}]:\n{exception}";
}