using MediatR;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Simple.Client.Interfaces;

namespace Neal.Twitter.Simple.Client.Events.Handlers;

public class SimpleTweetReceivedHandler : INotificationHandler<TweetReceivedNotification>
{
    #region Fields

    private readonly ISimpleProducerWrapper tweetRepositoryProducerWrapper;

    #endregion Fields

    public SimpleTweetReceivedHandler(ISimpleProducerWrapper tweetRepositoryProducer) =>
        this.tweetRepositoryProducerWrapper = tweetRepositoryProducer;

    #region INotificationHandler Implementation

    public async Task Handle(TweetReceivedNotification notification, CancellationToken cancellationToken) =>
        await this.tweetRepositoryProducerWrapper.ProduceAsync(notification.Tweet, cancellationToken);

    #endregion INotificationHandler Implementation
}