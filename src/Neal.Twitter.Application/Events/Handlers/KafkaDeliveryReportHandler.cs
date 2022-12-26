using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants;
using Neal.Twitter.Application.Events.Notifications;
using System.Text.Json;

namespace Neal.Twitter.Application.Events.Handlers;

public class KafkaDeliveryReportHandler : INotificationHandler<KafkaDeliveryResultNotification>
{
    #region Fields

    private readonly ILogger<KafkaDeliveryReportHandler> _logger;

    #endregion Fields

    public KafkaDeliveryReportHandler(ILogger<KafkaDeliveryReportHandler> logger) => this._logger = logger;

    public virtual Task Handle(KafkaDeliveryResultNotification notification, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        var deliveryReport = notification.DeliveryReport;

        try
        {
            if (deliveryReport.Error.Code != ErrorCode.NoError)
            {
                throw new Exception(JsonSerializer.Serialize(deliveryReport.Error));
            }
            else
            {
                this._logger.LogInformation(
                    KafkaHandlerLogMessages.PrintResult,
                    deliveryReport.Timestamp.UtcDateTime,
                    deliveryReport.Status,
                    deliveryReport.Topic);
            }
        }
        catch (Exception ex)
        {
            // Error returned from brokers.
            this._logger.LogCritical(
                ex,
                KafkaHandlerLogMessages.ProducerException,
                deliveryReport.Key,
                deliveryReport.Value,
                deliveryReport.Topic);
        }

        return Task.CompletedTask;
    }
}