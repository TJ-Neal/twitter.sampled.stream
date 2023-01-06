using LinqToTwitter.OAuth;
using Neal.Twitter.Application.Constants.Keys;
using Neal.Twitter.LinqToTwitter.Client.Interfaces;
using Neal.Twitter.LinqToTwitter.Client.Wrappers;

namespace Neal.Twitter.Infrastructure.StreamReader.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTwitterStreamConsumer(this IServiceCollection services, IConfigurationRoot? configuration)
    {
        // Create application authorizer and authenticate with Twitter for TwitterContext initialization.
        var twitterCredentialStore = new InMemoryCredentialStore
        {
            ConsumerKey = configuration
                ?.GetValue<string>($"{ApplicationConfigurationKeys.Twitter}:{ApplicationConfigurationKeys.ConsumerKey}"),
            ConsumerSecret = configuration
                ?.GetValue<string>($"{ApplicationConfigurationKeys.Twitter}:{ApplicationConfigurationKeys.ConsumerSecret}"),
        };
        var authorizer = new ApplicationOnlyAuthorizer { CredentialStore = twitterCredentialStore };

        services
            .AddSingleton<IAuthorizer>(authorizer)
            .AddSingleton(typeof(ILinqToTwitterWrapper), typeof(LinqToTwitterWrapper));

        return services;
    }
}