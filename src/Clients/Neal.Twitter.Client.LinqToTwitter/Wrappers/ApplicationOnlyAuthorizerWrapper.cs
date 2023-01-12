using LinqToTwitter.OAuth;
using Neal.Twitter.Client.LinqToTwitter.Interfaces;
using System.Net;

namespace Neal.Twitter.Client.LinqToTwitter.Wrappers;

/// <summary>
/// <inheritdoc cref="IApplicationOnlyAuthorizerWrapper"/>
/// </summary>
public class ApplicationOnlyAuthorizerWrapper : IApplicationOnlyAuthorizerWrapper
{
    #region Fields

    private readonly IAuthorizer applicationOnlyAuthorizer;

    #endregion Fields

    #region Properties

    public string? UserAgent
    {
        get => this.applicationOnlyAuthorizer.UserAgent;
        set => this.applicationOnlyAuthorizer.UserAgent = value;
    }

    public ICredentialStore? CredentialStore
    {
        get => this.applicationOnlyAuthorizer.CredentialStore;
        set => this.applicationOnlyAuthorizer.CredentialStore = value;
    }

    public IWebProxy? Proxy
    {
        get => this.applicationOnlyAuthorizer.Proxy;
        set => this.applicationOnlyAuthorizer.Proxy = value;
    }

    public bool SupportsCompression
    {
        get => this.applicationOnlyAuthorizer.SupportsCompression;
        set => this.applicationOnlyAuthorizer.SupportsCompression = value;
    }

    #endregion Properties

    public ApplicationOnlyAuthorizerWrapper(IAuthorizer applicationOnlyAuthorizer) =>
        this.applicationOnlyAuthorizer = applicationOnlyAuthorizer;

    #region IApplicationOnlyAuthorizerWrapper Implementations

    public virtual async Task AuthorizeAsync() =>
        await this.applicationOnlyAuthorizer.AuthorizeAsync();

    public string? GetAuthorizationString(string method, string oauthUrl, IDictionary<string, string> parameters) =>
        this.applicationOnlyAuthorizer.GetAuthorizationString(method, oauthUrl, parameters);

    #endregion IApplicationOnlyAuthorizerWrapper Implementations
}