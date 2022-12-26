﻿namespace Neal.Twitter.Application.Constants;

public struct SampledStreamLogMessages
{
    #region Fields

    public const string FatalException = "Twitter Stream ended in fatal exception.";

    public const string IOException = "Twitter Stream ended in fatal IO error.";

    public const string KeepAlive = "Keep-Alive signal received.";

    public const string NullTweet = "Null tweet object received.";

    public const string StreamError = "Error while streaming Twitter Sampled Stream API: {errorMessage}";

    #endregion Fields
}