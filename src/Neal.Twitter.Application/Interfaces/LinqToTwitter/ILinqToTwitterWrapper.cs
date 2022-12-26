using StreamContent = LinqToTwitter.StreamContent;

namespace Neal.Twitter.Application.Interfaces.LinqToTwitter;

public interface ILinqToTwitterWrapper
{
    Task StartAsync(Func<StreamContent, Task> callback, CancellationToken cancellationToken);

    Task ProcessStreamContent(StreamContent stream, string topic, CancellationToken cancellationToken);
}