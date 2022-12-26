using LinqToTwitter.OAuth;
using Neal.Twitter.Application.Interfaces.LinqToTwitter;
using System.Net;

namespace Neal.Twitter.Application.Wrappers.LinqToTwitter;

public class ApplicationOnlyAuthorizerWrapper : IApplicationOnlyAuthorizerWrapper
{
    #region Fields

    private readonly IAuthorizer _applicationOnlyAuthorizer;

    #endregion Fields

    #region Properties

    public string? UserAgent
    {
        get => this._applicationOnlyAuthorizer.UserAgent;
        set => this._applicationOnlyAuthorizer.UserAgent = value;
    }

    public ICredentialStore? CredentialStore
    {
        get => this._applicationOnlyAuthorizer.CredentialStore;
        set => this._applicationOnlyAuthorizer.CredentialStore = value;
    }

    public IWebProxy? Proxy
    {
        get => this._applicationOnlyAuthorizer.Proxy;
        set => this._applicationOnlyAuthorizer.Proxy = value;
    }

    public bool SupportsCompression
    {
        get => this._applicationOnlyAuthorizer.SupportsCompression;
        set => this._applicationOnlyAuthorizer.SupportsCompression = value;
    }

    #endregion Properties

    public ApplicationOnlyAuthorizerWrapper(IAuthorizer applicationOnlyAuthorizer) =>
        this._applicationOnlyAuthorizer = applicationOnlyAuthorizer;

    #region IApplicationOnlyAuthorizerWrapper Implementations

    public virtual async Task AuthorizeAsync() =>
        await this._applicationOnlyAuthorizer.AuthorizeAsync();

    public string? GetAuthorizationString(string method, string oauthUrl, IDictionary<string, string> parameters) =>
        this._applicationOnlyAuthorizer.GetAuthorizationString(method, oauthUrl, parameters);

    #endregion IApplicationOnlyAuthorizerWrapper Implementations
}