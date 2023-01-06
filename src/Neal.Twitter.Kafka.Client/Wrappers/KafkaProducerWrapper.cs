using Confluent.Kafka;
using Confluent.Kafka.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Core.Entities.Kafka;
using Neal.Twitter.Kafka.Client.Constants;
using Neal.Twitter.Kafka.Client.Events.Notifications;
using Neal.Twitter.Kafka.Client.Interfaces;

namespace Neal.Twitter.Kafka.Client.Wrappers;

public class KafkaProducerWrapper : IKafkaProducerWrapper, IDisposable
{
    #region Fields

    private readonly IMediator mediator;

    private readonly ILogger<KafkaProducerWrapper> logger;

    private readonly IProducer<string, string>? producer;

    #endregion Fields

    public KafkaProducerWrapper(
        IMediator mediator,
        ILogger<KafkaProducerWrapper> logger,
        IKafkaFactory kafkaFactory,
        IConfiguration configuration)
    {
        this.mediator = mediator;
        this.logger = logger;

        var kafkaSection = configuration
            ?.GetSection($"{ConfigurationKeys.Kafka}");

        var kafkaConfiguration = kafkaSection
            ?.GetSection($"{ConfigurationKeys.Configuration}")
            ?.AsEnumerable(makePathsRelative: true)
            .Select(pair => new KeyValuePair<string, string>(pair.Key, pair.Value ?? string.Empty));

        if (kafkaConfiguration is null
            || !kafkaConfiguration.Any(pair => string.Compare(pair.Key, ConfigurationKeys.BootstrapServers, true) != 0))
        {
            throw new KeyNotFoundException($"Key {ConfigurationKeys.BootstrapServers} was not found and is required.");
        }

        try
        {
            this.producer = kafkaFactory.CreateProducer<string, string>(kafkaConfiguration);
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
                this.mediator.Publish(new DeliveryReportNotification<string, string>(deliveryReport, cancellationToken));

                if (deliveryReport.Error.IsFatal)
                {
                    throw new ProduceException<string, string>(deliveryReport.Error, deliveryReport);
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
            this.logger.LogCritical(HandlerLogMessages.ProducerException, message.Message.Key, message.Message.Value, message.Topic, ex.Message);
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