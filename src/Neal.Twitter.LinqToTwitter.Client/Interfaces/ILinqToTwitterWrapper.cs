using StreamContent = LinqToTwitter.StreamContent;

namespace Neal.Twitter.LinqToTwitter.Client.Interfaces;

public interface ILinqToTwitterWrapper
{
    Task StartAsync(Func<StreamContent, Task> callback, CancellationToken cancellationToken);

    Task ProcessStreamContent(StreamContent stream, CancellationToken cancellationToken);
}