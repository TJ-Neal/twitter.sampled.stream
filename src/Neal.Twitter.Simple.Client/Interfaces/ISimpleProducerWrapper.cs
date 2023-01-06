using Neal.Twitter.Application.Interfaces;
using Neal.Twitter.Core.Entities.Twitter;

namespace Neal.Twitter.Simple.Client.Interfaces;

public interface ISimpleProducerWrapper : IProducerWrapper<TweetDto>
{
}