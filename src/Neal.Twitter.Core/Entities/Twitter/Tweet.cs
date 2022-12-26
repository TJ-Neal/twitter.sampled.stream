namespace Neal.Twitter.Core.Entities.Twitter;

public record Tweet(string? Id, string? Text, Hashtag[]? Hashtags);