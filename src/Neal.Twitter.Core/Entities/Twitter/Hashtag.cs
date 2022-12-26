namespace Neal.Twitter.Core.Entities.Twitter;

public record Hashtag(int Start, int End, string? Tag) : TwitterEntity(Start, End);