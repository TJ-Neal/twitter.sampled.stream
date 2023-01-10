namespace Neal.Twitter.Application.Constants.Messages;

/// <summary>
/// Represents strings for the various states of the status of the application.
/// </summary>
public struct ApplicationStatusMessages
{
    public const string FatalError = "Application failed unexpectedly.";

    public const string HostedServiceFinished = "Hosted service finished.";

    public const string HostedServiceStarting = "Hosted service starting...";

    public const string Started = "Application has been started.";

    public const string Stopped = "Application has been stopped.";

    public const string TweetCount = "{source} running. Currently has {tweetsCount} tweet(s) and {hashtagsCount} hashtag(s).";
}