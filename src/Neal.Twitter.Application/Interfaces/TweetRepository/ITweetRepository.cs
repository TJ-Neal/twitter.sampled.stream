using Neal.Twitter.Core.Entities.Twitter;

namespace Neal.Twitter.Application.Interfaces.TweetRepository;

public interface ITweetRepository : IDisposable
{
    Task AddRecordsAsync(IEnumerable<TweetDto> tweets);

    Task<List<TweetDto>> GetAllTweetsAsync(int? page, int? pagesize);

    Task<long> GetCountAsync();

    Task<IEnumerable<KeyValuePair<string, int>>> GetTopHashtags(int top = 10);
}