using Neal.Twitter.Core.Entities.Configuration;
using Neal.Twitter.Core.Entities.Twitter;

namespace Neal.Twitter.Application.Interfaces.TweetRepository;

/// <summary>
/// Represents the interface for interacting with the data repositories for the Tweet stream from Twitter.
/// </summary>
public interface ITweetRepository : IDisposable
{
    Task AddRecordsAsync(IEnumerable<TweetDto> tweets);

    Task<List<TweetDto>> GetAllTweetsAsync(Pagination pagination);

    Task<long> GetCountAsync();

    Task<IEnumerable<KeyValuePair<string, int>>> GetTopHashtags(int top = 10);
}