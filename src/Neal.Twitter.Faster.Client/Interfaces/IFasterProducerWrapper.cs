using Neal.Twitter.Application.Interfaces;
using Neal.Twitter.Core.Entities.Twitter;

namespace Neal.Twitter.Faster.Client.Interfaces;

public interface IFasterProducerWrapper : IProducerWrapper<TweetDto>
{
}