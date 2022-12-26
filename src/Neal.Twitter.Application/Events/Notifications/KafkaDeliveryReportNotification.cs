using Confluent.Kafka;
using MediatR;

namespace Neal.Twitter.Application.Events.Notifications;

public record KafkaDeliveryResultNotification(
    DeliveryReport<string, string> DeliveryReport,
    CancellationToken CancellationToken) : INotification;