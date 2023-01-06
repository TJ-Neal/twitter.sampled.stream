using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Kafka.Client.Constants;
using Neal.Twitter.Kafka.Client.Events.Notifications;
using System.Text.Json;

namespace Neal.Twitter.Kafka.Client.Events.Handlers;

public class DeliveryReportHandler : INotificationHandler<DeliveryReportNotification<string, string>>
{
    #region Fields

    private readonly ILogger<DeliveryReportHandler> logger;

    #endregion Fields

    public DeliveryReportHandler(ILogger<DeliveryReportHandler> logger) => this.logger = logger;

    public virtual Task Handle(DeliveryReportNotification<string, string> notification, CancellationToken cancellationToken)
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
                this.logger.LogInformation(
                    HandlerLogMessages.PrintResult,
                    deliveryReport.Timestamp.UtcDateTime,
                    deliveryReport.Status,
                    deliveryReport.Topic);
            }
        }
        catch (Exception ex)
        {
            // Error returned from brokers.
            this.logger.LogCritical(
                ex,
                HandlerLogMessages.ProducerException,
                deliveryReport.Key,
                deliveryReport.Value,
                deliveryReport.Topic,
                ex.Message);
        }

        return Task.CompletedTask;
    }
}