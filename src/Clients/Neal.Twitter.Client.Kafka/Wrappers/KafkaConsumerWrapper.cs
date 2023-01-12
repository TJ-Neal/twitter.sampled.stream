using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Utilities;
using Neal.Twitter.Client.Kafka.Constants;
using Neal.Twitter.Client.Kafka.Interfaces;
using Neal.Twitter.Core.Entities.Configuration;
using Neal.Twitter.Core.Entities.Twitter;
using System.Net.Http.Json;
using System.Text.Json;

namespace Neal.Twitter.Client.Kafka.Wrappers;

/// <summary>
/// Represents a wrapper for interacting with the Kafka consumer client and Kafka consuming message logs from a Kafka message broker.
/// </summary>
public class KafkaConsumerWrapper : IKafkaConsumerWrapper<string, string>
{
    private static bool hasFaulted = false;

    private readonly IConsumer<string, string> consumer;

    private readonly ILogger<KafkaConsumerWrapper> logger;

    private readonly IHttpClientFactory clientFactory;

    private readonly WrapperConfiguration<ConsumerConfig> configuration;

    public KafkaConsumerWrapper(WrapperConfiguration<ConsumerConfig> wrapperConfiguration, ILogger<KafkaConsumerWrapper> logger, IHttpClientFactory httpClientFactory)
    {
        this.logger = logger;
        this.clientFactory = httpClientFactory;
        this.configuration = wrapperConfiguration;

        if (this.configuration is null
            || string.IsNullOrEmpty(this.configuration?.ClientConfig?.BootstrapServers))
        {
            throw new KeyNotFoundException($"Key {nameof(this.configuration.ClientConfig.BootstrapServers)} was not found and is required.");
        }

        this.configuration.ClientConfig.GroupId = new Guid().ToString(); // Create a random consumer group so messages can be replayed from offset without special configuration
        this.consumer =
            new ConsumerBuilder<string, string>(this.configuration.ClientConfig)
            .Build();
        this.consumer.Subscribe(this.configuration.Topic);
        this.logger.LogInformation(WrapperLogMessages.Subscribed, wrapperConfiguration.Topic);
    }

    public async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(this.configuration.BaseUrl))
        {
            throw new InvalidOperationException(CommonLogMessages.InvalidConfiguration);
        }

        // TODO: Create a timer for resetting faulted so producers can retry after t time
        while (this.configuration.Enabled
            && !hasFaulted
            && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                var tweet = await this.GetNextSubscriptionResult(cancellationToken);

                if (!hasFaulted && tweet is not null)
                {
                    int retries = 0;

                    while (retries < RetryUtilities.MaxRetries)
                    {
                        try
                        {
                            using var client = clientFactory.CreateClient();

                            var postResult = await client.PostAsJsonAsync(this.configuration.BaseUrl, new List<TweetDto> { tweet }, cancellationToken);

                            if (postResult is null || !postResult.IsSuccessStatusCode)
                            {
                                this.logger.LogError(HandlerLogMessages.ConsumerPostError, postResult?.StatusCode, postResult?.Content);
                            }
                            else
                            {
                                this.logger.LogDebug(HandlerLogMessages.PrintConsumeResult, DateTime.Now, postResult.StatusCode, tweet.Id);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            this.logger.LogCritical(HandlerLogMessages.ProducerException, ex.Message);

                            // Exit loop if exception is not transient
                            if (!RetryUtilities.ExceptionIsTransient(ex))
                            {
                                hasFaulted = true;
                                break;
                            }
                        }

                        await Task.Delay(RetryUtilities.GetDelay(++retries), cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(HandlerLogMessages.ConsumerException, ex.Message);
            }
        }
    }

    public void Dispose()
    {
        this.logger.LogInformation(CommonLogMessages.Disposing, nameof(KafkaConsumerWrapper));
        this.logger.LogInformation(CommonLogMessages.Disposing, nameof(this.consumer));

        try
        {
            if (this.consumer is not null)
            {
                this.consumer.Unsubscribe();
                this.consumer.Close();
                this.consumer.Dispose();
            }
        }
        catch (Exception ex)
        {
            this.logger
                .LogError(ExceptionMessages.DisposeException, nameof(this.consumer), ex.Message);
        }

        GC.SuppressFinalize(this);
    }

    private async Task<TweetDto?> GetNextSubscriptionResult(CancellationToken cancellationToken)
    {
        try
        {
            int retries = 0;

            while (retries < RetryUtilities.MaxRetries)
            {
                var consumeResult = await Task.Run(() => this.consumer.Consume(cancellationToken));

                if (consumeResult is not null)
                {
                    var tweet = JsonSerializer.Deserialize<TweetDto>(consumeResult.Message.Value);

                    if (tweet is not null)
                    {
                        this.logger.LogDebug(
                            HandlerLogMessages.PrintConsumeResult,
                            DateTime.Now.ToUniversalTime(),
                            consumeResult.Message.Key,
                            consumeResult.Message.Timestamp.UtcDateTime);

                        return tweet;
                    }
                }

                await Task.Delay(RetryUtilities.GetDelay(++retries), cancellationToken);
            }

            hasFaulted = retries >= RetryUtilities.MaxRetries;
        }
        catch (Exception ex)
        {
            this.logger.LogCritical(HandlerLogMessages.ConsumerException, ex.Message);
        }

        return default;
    }
}