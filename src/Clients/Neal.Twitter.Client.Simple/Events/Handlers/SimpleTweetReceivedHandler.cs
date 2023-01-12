using MediatR;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Client.Simple.Interfaces;

namespace Neal.Twitter.Client.Simple.Events.Handlers;

/// <summary>
/// Client handler for the Simple repository for when a Tweet event is raised.
/// </summary>
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

    public override int GetHashCode() => base.GetHashCode();
}