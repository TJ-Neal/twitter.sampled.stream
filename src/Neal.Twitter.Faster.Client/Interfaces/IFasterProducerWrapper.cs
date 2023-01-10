using Neal.Twitter.Application.Interfaces;
using Neal.Twitter.Core.Entities.Twitter;

namespace Neal.Twitter.Faster.Client.Interfaces;

/// <summary>
/// Represents a wrapper for events produced for the FasterKV client and repository.
/// </summary>
public interface IFasterProducerWrapper : IProducerWrapper<TweetDto>
{
}