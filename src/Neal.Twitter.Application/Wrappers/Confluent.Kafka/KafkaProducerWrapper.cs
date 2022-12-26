using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Application.Interfaces.Confluent.Kafka;

namespace Neal.Twitter.Application.Wrappers.Confluent.Kafka;

public class KafkaProducerWrapper : IKafkaProducerWrapper
{
    #region Fields

    private readonly IEnumerable<KeyValuePair<string, string?>> _kafkaConfiguration;

    private readonly IMediator _mediator;

    private readonly ILogger<KafkaProducerWrapper> _logger;

    private readonly IProducer<string, string>? _producer;

    #endregion Fields

    public KafkaProducerWrapper(
        IMediator mediator,
        ILogger<KafkaProducerWrapper> logger,
        IEnumerable<KeyValuePair<string, string?>> kafkaConfiguration)
    {
        this._mediator = mediator;
        this._logger = logger;
        this._kafkaConfiguration = kafkaConfiguration;

        var builder = new ProducerBuilder<string, string>(this._kafkaConfiguration);

        this._producer = builder.Build();
    }

    public void Flush()
    {
        this._producer?.Flush();
        this._logger.LogInformation("Kafka producer is being flushed.");
    }

    public Task Produce(string topic, Message<string, string> message, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        try
        {
            var callback = delegate (DeliveryReport<string, string> deliveryReport)
            {
                this._mediator.Publish(new KafkaDeliveryResultNotification(deliveryReport, cancellationToken));
            };

            this._producer?
                .Produce(topic, message, callback);
        }
        catch (Exception ex)
        {
            // Cannot produce broker message.
            this._logger
                .LogCritical(
                    ex,
                    KafkaHandlerLogMessages.ProducerException,
                    message.Key,
                    message.Value,
                    topic);
        }

        return Task.CompletedTask;
    }
}