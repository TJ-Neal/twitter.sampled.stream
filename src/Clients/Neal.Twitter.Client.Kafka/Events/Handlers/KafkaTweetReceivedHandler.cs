using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Client.Kafka.Constants;
using Neal.Twitter.Client.Kafka.Interfaces;
using Neal.Twitter.Core.Entities.Configuration;
using Neal.Twitter.Core.Entities.Kafka;
using Neal.Twitter.Core.Entities.Twitter;

namespace Neal.Twitter.Client.Kafka.Events.Handlers;

/// <summary>
/// Implementation of an event handler for received tweets to capture them and publish them to Kafka
/// </summary>
public class KafkaTweetReceivedHandler : INotificationHandler<TweetReceivedNotification>
{
    #region Fields

    private readonly string topic;

    private readonly IKafkaProducerWrapper kafkaProducerWrapper;

    private readonly ILogger<KafkaTweetReceivedHandler> logger;

    #endregion Fields

    public KafkaTweetReceivedHandler(IKafkaProducerWrapper kafkaProducer, ILogger<KafkaTweetReceivedHandler> logger, WrapperConfiguration<ProducerConfig> wrapperConfiguration)
    {
        this.topic = string.IsNullOrEmpty(wrapperConfiguration.Topic)
                ? throw new KafkaException(ErrorCode.Local_UnknownTopic)
                : wrapperConfiguration.Topic;
        this.kafkaProducerWrapper = kafkaProducer;
        this.logger = logger;
    }

    #region INotificationHandler Implementation

    public void Dispose() => this.kafkaProducerWrapper.Flush();

    /// <summary>
    /// Handle when a <seealso cref="TweetReceivedNotification"/> notification is received, publishing to the Kafka stream.
    /// </summary>
    public async Task Handle(TweetReceivedNotification notification, CancellationToken cancellationToken)
    {
        if (notification is null)
        {
            this.logger.LogDebug(HandlerLogMessages.NullNotification);

            return;
        }

        if (notification.Tweet is null)
        {
            this.logger.LogDebug(HandlerLogMessages.NullTweet);

            return;
        }

        if (notification.Tweet.Id is null || string.IsNullOrWhiteSpace(notification.Tweet.Id))
        {
            this.logger.LogDebug(HandlerLogMessages.NullTweetId);

            return;
        }

        await this.kafkaProducerWrapper
            .ProduceAsync(
                new KafkaProducerMessage(
                    new Message<string, TweetDto>
                    {
                        Key = notification.Tweet.Id.ToString()!,
                        Value = notification.Tweet
                    },
                    this.topic
                ),
                cancellationToken);
    }

    #endregion INotificationHandler Implementation
}