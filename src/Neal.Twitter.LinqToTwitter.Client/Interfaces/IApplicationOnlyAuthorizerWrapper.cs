using LinqToTwitter.OAuth;

namespace Neal.Twitter.LinqToTwitter.Client.Interfaces;

public interface IApplicationOnlyAuthorizerWrapper : IAuthorizer
{
    new Task AuthorizeAsync();
}