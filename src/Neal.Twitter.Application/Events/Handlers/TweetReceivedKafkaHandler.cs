using Confluent.Kafka;
using MediatR;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Application.Interfaces.Confluent.Kafka;
using System.Text.Json;

namespace Neal.Twitter.Application.Events.Handlers;

/// <summary>
/// Implementation of an event handler for received tweets to capture them and publish them to Kafka
/// </summary>
public class TweetReceivedKafkaHandler : INotificationHandler<TweetReceivedNotification>
{
    #region Fields

    private readonly IKafkaProducerWrapper _kafkaProducerWrapper;

    #endregion Fields

    #region Public Constructors

    public TweetReceivedKafkaHandler(IKafkaProducerWrapper kafkaProducer) => this._kafkaProducerWrapper = kafkaProducer;

    #endregion Public Constructors

    #region INotificationHandler Implementation

    public void Dispose() => this._kafkaProducerWrapper.Flush();

    /// <summary>
    /// Handle when a <seealso cref="TweetReceivedNotification"/> notification is received, publishing to the Kafka stream.
    /// </summary>
    public Task Handle(TweetReceivedNotification notification, CancellationToken cancellationToken) =>
        this._kafkaProducerWrapper
            .Produce(
                notification.Topic,
                new Message<string, string>
                {
                    Key = notification.Tweet.ID ?? Guid.NewGuid().ToString(),
                    Value = JsonSerializer.Serialize(notification.Tweet)
                },
                cancellationToken);

    #endregion INotificationHandler Implementation
}