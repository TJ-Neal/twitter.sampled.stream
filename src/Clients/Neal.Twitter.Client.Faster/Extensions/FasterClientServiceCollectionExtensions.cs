using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Neal.Twitter.Client.Faster.Events.Handlers;
using Neal.Twitter.Client.Faster.Interfaces;
using Neal.Twitter.Client.Faster.Wrappers;
using Neal.Twitter.Core.Entities.Configuration;

namespace Neal.Twitter.Client.Faster.Extensions;

/// <summary>
/// Add the required types for dependency injection when the FasterKV client and repository are enabled according to the provided <see cref="FasterConfiguration"/>.
/// </summary>
public static class FasterClientServiceCollectionExtensions
{
    public static IServiceCollection AddFasterRepositoryHandlerIfEnabled(this IServiceCollection services, FasterConfiguration fasterConfiguration)
    {
        if (!fasterConfiguration.Enabled)
        {
            return services;
        }

        services
            .AddHttpClient()
            .AddSingleton(fasterConfiguration)
            .AddSingleton(typeof(IFasterProducerWrapper), typeof(FasterProducerWrapper))
            .AddMediatR(typeof(FasterTweetReceivedHandler));

        return services;
    }
}