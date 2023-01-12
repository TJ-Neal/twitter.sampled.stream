using Confluent.Kafka;
using Confluent.Kafka.DependencyInjection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Neal.Twitter.Client.Kafka.Events.Handlers;
using Neal.Twitter.Client.Kafka.Interfaces;
using Neal.Twitter.Client.Kafka.Wrappers;
using Neal.Twitter.Core.Entities.Configuration;

namespace Neal.Twitter.Client.Kafka.Extensions;

/// <summary>
/// Add the required types for dependency injection when the Kafka client and repository are enabled according to the provided <see cref="KafkaProducerWrapperConfiguration"/>.
/// </summary>
public static class KafkaClientServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaHandlerIfEnabled(
        this IServiceCollection services,
        WrapperConfiguration<ProducerConfig> kafkaProducerConfiguration)
    {
        if (!kafkaProducerConfiguration.Enabled)
        {
            return services;
        }

        services
            .AddKafkaClient()
            .AddSingleton(kafkaProducerConfiguration)
            .AddSingleton(typeof(IKafkaProducerWrapper), typeof(KafkaProducerWrapper))
            .AddMediatR(typeof(KafkaTweetReceivedHandler));

        return services;
    }
}