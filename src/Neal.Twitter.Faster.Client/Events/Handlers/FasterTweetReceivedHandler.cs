using MediatR;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Faster.Client.Interfaces;

namespace Neal.Twitter.Faster.Client.Events.Handlers;

public class FasterTweetReceivedHandler : INotificationHandler<TweetReceivedNotification>
{
    private readonly IFasterProducerWrapper tweetRepositoryProducerWrapper;

    public FasterTweetReceivedHandler(IFasterProducerWrapper tweetRepositoryProducer) =>
        this.tweetRepositoryProducerWrapper = tweetRepositoryProducer;

    public async Task Handle(TweetReceivedNotification notification, CancellationToken cancellationToken) =>
        await this.tweetRepositoryProducerWrapper
            .ProduceAsync(notification.Tweet, cancellationToken);

    public override int GetHashCode() => base.GetHashCode();
}