namespace Neal.Twitter.Application.Constants.Api;

public struct ApiStrings
{
    public const string BaseRoute = "/api";

    public const string TweetsRoute = "/tweets";

    public const string GetTweetsName = "GetAllTweets";

    public const string GetTweetsDescription = "View a list of received Tweets. Maximum of 1000 per page";

    public const string PostTweetsName = "AddTweets";

    public const string PostTweetsDescription = "Add list of additional Tweets to the repository.";

    public const string CountRoute = "/tweets/count";

    public const string GetCountName = "GetTweetsCount";

    public const string GetCountDescription = "Get the current count of Tweets received.";

    public const string HashtagsRoute = "/tweets/hashtags";

    public const string GetHashtagsName = "GetTopHashtags";

    public const string GetHashtagsDescription = "View the top X Hashtags used. Defaults to 10.";
}