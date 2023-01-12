namespace Neal.Twitter.Application.Constants.Messages;

/// <summary>
/// Represents various strings for used in exception messages used within the application.
/// </summary>
public struct ExceptionMessages
{
    public const string DisposeException = "There was an exception while disposing [{className}]:\n{exception}";

    public const string ErrorDuringLoop = "Failed during loop executions\n{exception}";

    public const string GenericException = "Exception encountered during execution: {message}";

    public const string InstantiationError = "Exception creating {name}\n{ex}";

    public const string KeyNotFound = "Unable to read key.";

    public const string RequiredKeyNotFound = "Key {name} was not found and is required.";

    public const string SerializationError = "{name} is not valid cannot be serialized.";
}