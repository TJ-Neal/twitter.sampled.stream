using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Neal.Twitter.Client.Simple.Events.Handlers;
using Neal.Twitter.Client.Simple.Interfaces;
using Neal.Twitter.Client.Simple.Wrappers;
using Neal.Twitter.Core.Entities.Configuration;

namespace Neal.Twitter.Client.Simple.Extensions;

/// <summary>
/// Add the required types for dependency injection when the Simple client and repository are enabled according to the provided <see cref="SimpleConfiguration"/>.
/// </summary>
public static class SimpleClientServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleRepositoryHandlerIfEnabled(this IServiceCollection services, SimpleConfiguration simpleConfiguration)
    {
        if (!simpleConfiguration.Enabled)
        {
            return services;
        }

        services
            .AddHttpClient()
            .AddSingleton(simpleConfiguration)
            .AddSingleton(typeof(ISimpleProducerWrapper), typeof(SimpleProducerWrapper))
            .AddMediatR(typeof(SimpleTweetReceivedHandler));

        return services;
    }
}