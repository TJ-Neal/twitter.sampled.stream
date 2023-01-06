using System.Text.Json.Serialization;

namespace Neal.Twitter.Core.Entities.Twitter;

public class Hashtag
{
    public int Start { get; init; }

    public int End { get; init; }

    public string? Tag { get; init; } = null;

    [JsonConstructor]
    public Hashtag(int start, int end, string? tag)
    {
        this.Start = start;
        this.End = end;
        this.Tag = tag;
    }
}