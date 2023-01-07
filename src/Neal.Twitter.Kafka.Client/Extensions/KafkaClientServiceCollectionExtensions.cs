using Confluent.Kafka.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neal.Twitter.Kafka.Client.Constants;
using Neal.Twitter.Kafka.Client.Events.Handlers;
using Neal.Twitter.Kafka.Client.Interfaces;
using Neal.Twitter.Kafka.Client.Wrappers;

namespace Neal.Twitter.Kafka.Client.Extensions;

public static class KafkaClientServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaHandlerIfEnabled(this IServiceCollection services, IConfiguration configuration, List<Type> mediatrTypes)
    {
        // TODO: Use Kafka model to convert this
        bool isEnabled = configuration
            ?.GetSection(ConfigurationKeys.Kafka)
            ?.GetValue<bool>(ConfigurationKeys.Enabled)
                ?? false;

        if (!isEnabled)
        {
            return services;
        }

        services
            .AddKafkaClient()
            .AddSingleton(typeof(IKafkaProducerWrapper), typeof(KafkaProducerWrapper));

        mediatrTypes
            .Add(typeof(KafkaTweetReceivedHandler));

        return services;
    }
}