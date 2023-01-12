using StreamContent = LinqToTwitter.StreamContent;

namespace Neal.Twitter.Client.LinqToTwitter.Interfaces;

/// Represents a wrapper for interacting with <see cref="LinqToTwitter"/>.
/// </summary>
public interface ILinqToTwitterWrapper
{
    Task StartAsync(Func<StreamContent, Task> callback, CancellationToken cancellationToken);

    Task ProcessStreamContent(StreamContent stream, CancellationToken cancellationToken);
}