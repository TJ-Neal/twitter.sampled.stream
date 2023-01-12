using Neal.Twitter.Application.Interfaces;
using Neal.Twitter.Core.Entities.Twitter;

namespace Neal.Twitter.Client.Simple.Interfaces;

/// <summary>
/// Represents a wrapper for events produced for the Simple client and repository.
/// </summary>
public interface ISimpleProducerWrapper : IProducerWrapper<TweetDto>
{
}