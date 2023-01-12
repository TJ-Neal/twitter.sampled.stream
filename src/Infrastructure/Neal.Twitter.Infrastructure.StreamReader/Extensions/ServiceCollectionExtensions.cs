using LinqToTwitter.OAuth;
using Neal.Twitter.LinqToTwitter.Client.Interfaces;
using Neal.Twitter.LinqToTwitter.Client.Wrappers;

namespace Neal.Twitter.Infrastructure.StreamReader.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTwitterStreamConsumer(this IServiceCollection services, InMemoryCredentialStore inMemoryCredentialStore)
    {
        var authorizer = new ApplicationOnlyAuthorizer { CredentialStore = inMemoryCredentialStore };

        services
            .AddSingleton<IAuthorizer>(authorizer)
            .AddSingleton(typeof(ILinqToTwitterWrapper), typeof(LinqToTwitterWrapper));

        return services;
    }
}