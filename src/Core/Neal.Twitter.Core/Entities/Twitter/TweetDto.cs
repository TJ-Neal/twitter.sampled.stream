using LinqToTwitter;
using System.Text.Json.Serialization;

namespace Neal.Twitter.Core.Entities.Twitter;

public class TweetDto
{
    public string Id { get; init; }

    public string Text { get; init; }

    public Hashtag[]? Hashtags { get; init; }

    public TweetDto(Tweet tweet) : this(
            tweet?.ID
                ?? throw new ArgumentException($"Invalid identifier for {nameof(tweet)}."),
            tweet.Text
                ?? string.Empty,
            tweet.Entities?
                .Hashtags?
                .Select(hastag => new Hashtag(hastag.Start, hastag.End, hastag.Tag))
                .ToArray()
        )
    {
    }

    [JsonConstructor]
    public TweetDto(string id, string text, Hashtag[]? hashtags)
    {
        this.Id = id;
        this.Text = text;
        this.Hashtags = hashtags;
    }
}