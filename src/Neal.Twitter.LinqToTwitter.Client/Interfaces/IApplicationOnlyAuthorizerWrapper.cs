﻿using LinqToTwitter.OAuth;

namespace Neal.Twitter.LinqToTwitter.Client.Interfaces;

/// <summary>
/// Represents a wrapper for interacting with the <see cref="ApplicationOnlyAuthorizer"/> from <see cref="LinqToTwitter"/>.
/// </summary>
public interface IApplicationOnlyAuthorizerWrapper : IAuthorizer
{
    new Task AuthorizeAsync();
}