using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neal.Twitter.Application.Constants.Keys;
using Neal.Twitter.Faster.Client.Events.Handlers;
using Neal.Twitter.Faster.Client.Interfaces;
using Neal.Twitter.Faster.Client.Wrappers;

namespace Neal.Twitter.Faster.Client.Extensions;

public static class FasterClientServiceCollectionExtensions
{
    public static IServiceCollection AddFasterRepositoryHandlerIfEnabled(this IServiceCollection services, IConfiguration configuration, List<Type> mediatrTypes)
    {
        bool isEnabled = configuration
            ?.GetSection(ApplicationConfigurationKeys.Faster)
            ?.GetValue<bool>(ApplicationConfigurationKeys.Enabled)
                ?? false;

        if (!isEnabled)
        {
            return services;
        }

        services
            .AddHttpClient()
            .AddSingleton(typeof(IFasterProducerWrapper), typeof(FasterProducerWrapper));

        mediatrTypes
            .Add(typeof(FasterTweetReceivedHandler));

        return services;
    }
}