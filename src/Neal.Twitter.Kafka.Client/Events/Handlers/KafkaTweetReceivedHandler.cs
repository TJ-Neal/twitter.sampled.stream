using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Core.Entities.Kafka;
using Neal.Twitter.Core.Entities.Twitter;
using Neal.Twitter.Kafka.Client.Constants;
using Neal.Twitter.Kafka.Client.Interfaces;

namespace Neal.Twitter.Kafka.Client.Events.Handlers;

/// <summary>
/// Implementation of an event handler for received tweets to capture them and publish them to Kafka
/// </summary>
public class KafkaTweetReceivedHandler : INotificationHandler<TweetReceivedNotification>
{
    #region Fields

    private readonly bool isEnabled;

    private readonly string topic;

    private readonly IKafkaProducerWrapper kafkaProducerWrapper;

    private readonly ILogger<KafkaTweetReceivedHandler> logger;

    #endregion Fields

    public KafkaTweetReceivedHandler(IConfiguration configuration, IKafkaProducerWrapper kafkaProducer, ILogger<KafkaTweetReceivedHandler> logger)
    {
        this.isEnabled = configuration
            ?.GetSection(ConfigurationKeys.Kafka)
            ?.GetValue<bool>(ConfigurationKeys.Enabled)
                ?? false;
        this.topic = configuration
            ?.GetSection(ConfigurationKeys.Kafka)
            ?.GetValue<string>(ConfigurationKeys.Topic)
                ?? throw new KafkaException(ErrorCode.Local_UnknownTopic);
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
            this.logger.LogInformation("Notification that is null received; unable to process.");

            return;
        }

        if (notification.Tweet is null)
        {
            this.logger.LogInformation("Notification with null tweet received; unable to process.");

            return;
        }

        if (notification.Tweet is null)
        {
            this.logger.LogInformation("Tweet with no ID received; unable to process.");

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