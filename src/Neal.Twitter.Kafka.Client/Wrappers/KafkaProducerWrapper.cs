using Confluent.Kafka;
using Confluent.Kafka.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Core.Entities.Configuration;
using Neal.Twitter.Core.Entities.Kafka;
using Neal.Twitter.Kafka.Client.Constants;
using Neal.Twitter.Kafka.Client.Interfaces;

namespace Neal.Twitter.Kafka.Client.Wrappers;

/// <summary>
/// Represents a wrapper for interacting with the Kafka producer client and Kafka production of message logs from a Kafka message broker.
/// </summary>
public class KafkaProducerWrapper : IKafkaProducerWrapper, IDisposable
{
    #region Fields

    private readonly ILogger<KafkaProducerWrapper> logger;

    private readonly IProducer<string, string>? producer;

    private int producedCount = 0;

    #endregion Fields

    public KafkaProducerWrapper(
        ILogger<KafkaProducerWrapper> logger,
        IKafkaFactory kafkaFactory,
        WrapperConfiguration<ProducerConfig> wrapperConfiguration)
    {
        this.logger = logger;

        if (wrapperConfiguration is null
            || string.IsNullOrEmpty(wrapperConfiguration?.ClientConfig?.BootstrapServers))
        {
            throw new KeyNotFoundException($"Key {nameof(wrapperConfiguration.ClientConfig.BootstrapServers)} was not found and is required.");
        }

        try
        {
            this.producer = kafkaFactory.CreateProducer<string, string>(wrapperConfiguration.ClientConfig);
        }
        catch (Exception ex)
        {
            this.logger.LogCritical("Error creating Kafka Producer\n{ex}", ex);
        }
    }

    public Task ProduceAsync(KafkaProducerMessage message, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        try
        {
            var callback = delegate (DeliveryReport<string, string> deliveryReport)
            {
                if (deliveryReport.Error.IsFatal)
                {
                    throw new ProduceException<string, string>(deliveryReport.Error, deliveryReport);
                }
                else
                {
                    this.producedCount++;
                    this.logger.LogDebug(HandlerLogMessages.PrintProducerResult, deliveryReport.Timestamp, deliveryReport.Status, deliveryReport.Topic, this.producedCount);

                    if ((this.producedCount / 100) % 10 == 0)
                    {
                        this.logger.LogInformation("{count} messages posted.", this.producedCount);
                    }
                }
            };

            this.producer?
                .Produce(
                    message.Topic,
                    message.Message,
                    callback);
        }
        catch (Exception ex)
        {
            this.logger.LogCritical(
                HandlerLogMessages.ProducerException,
                message.Message.Key,
                message.Message.Value,
                message.Topic,
                ex.Message);
        }

        return Task.CompletedTask;
    }

    public void Flush()
    {
        this.producer?.Flush();
        this.logger.LogInformation("Kafka producer is being flushed.");
    }

    public void Dispose()
    {
        this.logger.LogInformation("KafkaProducerWrapper is being disposed.");
        this.producer?.Dispose();

        GC.SuppressFinalize(this);
    }
}