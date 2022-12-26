using LinqToTwitter.OAuth;

namespace Neal.Twitter.Application.Interfaces.LinqToTwitter;

internal interface IApplicationOnlyAuthorizerWrapper : IAuthorizer
{
    new Task AuthorizeAsync();
}