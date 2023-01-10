using Confluent.Kafka;
using Confluent.Kafka.DependencyInjection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Neal.Twitter.Core.Entities.Configuration;
using Neal.Twitter.Kafka.Client.Events.Handlers;
using Neal.Twitter.Kafka.Client.Interfaces;
using Neal.Twitter.Kafka.Client.Wrappers;

namespace Neal.Twitter.Kafka.Client.Extensions;

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