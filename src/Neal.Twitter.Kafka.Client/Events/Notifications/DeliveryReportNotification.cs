using Confluent.Kafka;
using MediatR;

namespace Neal.Twitter.Kafka.Client.Events.Notifications;

public record DeliveryReportNotification<TKey, TValue>(
    DeliveryReport<TKey, TValue> DeliveryReport,
    CancellationToken CancellationToken) : INotification;