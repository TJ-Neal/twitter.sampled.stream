using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neal.Twitter.Application.Constants.Keys;
using Neal.Twitter.Simple.Client.Events.Handlers;
using Neal.Twitter.Simple.Client.Interfaces;
using Neal.Twitter.Simple.Client.Wrappers;

namespace Neal.Twitter.Simple.Client.Extensions;

public static class SimpleClientServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleRepositoryHandlerIfEnabled(this IServiceCollection services, IConfiguration configuration, List<Type> mediatrTypes)
    {
        // TODO: Create a model to do this
        bool isEnabled = configuration
            ?.GetSection(ApplicationConfigurationKeys.Simple)
            ?.GetValue<bool>(ApplicationConfigurationKeys.Enabled)
                ?? false;

        if (!isEnabled)
        {
            return services;
        }

        services
            .AddHttpClient()
            .AddSingleton(typeof(ISimpleProducerWrapper), typeof(SimpleProducerWrapper));

        mediatrTypes
            .Add(typeof(SimpleTweetReceivedHandler));

        return services;
    }
}