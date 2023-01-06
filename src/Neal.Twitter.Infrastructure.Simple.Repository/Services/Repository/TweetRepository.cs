using Microsoft.Extensions.Caching.Memory;
using Neal.Twitter.Application.Constants.Keys;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Interfaces.TweetRepository;
using Neal.Twitter.Core.Entities.Twitter;
using System.Collections.Concurrent;

namespace Neal.Twitter.Infrastructure.Simple.Repository.Services.Repository;

public class TweetRepository : ITweetRepository
{
    #region Fields

    private readonly IMemoryCache memoryCache;

    private readonly ILogger<TweetRepository> logger;

    private int defaultPageSize;

    private int maxPageSize;

    #endregion Fields

    #region Properties

    private ConcurrentDictionary<string, TweetDto> Tweets
    {
        get
        {
            var cachedValue = this.memoryCache
                .GetOrCreate<ConcurrentDictionary<string, TweetDto>>(
                    CacheKeys.TweetsRepository,
                    cacheEntry =>
                    {
                        cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                        return new();
                    });

            return cachedValue ?? new();
        }
    }

    private ConcurrentDictionary<string, int> HashTagCounts
    {
        get
        {
            var cachedValue = this.memoryCache
                .GetOrCreate<ConcurrentDictionary<string, int>>(
                    CacheKeys.HashtagsRepository,
                    cacheEntry =>
                    {
                        cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                        return new();
                    });

            return cachedValue ?? new();
        }
    }

    #endregion Properties

    public TweetRepository(IMemoryCache memoryCache, ILogger<TweetRepository> logger, IConfiguration configuration)
    {
        this.memoryCache = memoryCache;
        this.logger = logger;
        this.InitializePagination(configuration);
    }

    public Task AddRecordsAsync(IEnumerable<TweetDto> tweets)
    {
        if (tweets is null)
        {
            return Task.CompletedTask;
        }

        foreach (var tweet in tweets)
        {
            if (this.Tweets.TryAdd(tweet.Id, tweet))
            {
                if (tweet.Hashtags is not null)
                {
                    foreach (var hashTag in tweet.Hashtags!)
                    {
                        if (!string.IsNullOrEmpty(hashTag.Tag))
                        {
                            this.HashTagCounts.AddOrUpdate(hashTag.Tag, 1, (key, old) => ++old);
                        }
                    }
                }
            }
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        this.logger.LogInformation(CommonLogMessages.Disposing, nameof(TweetRepository));

        this.Tweets.Clear();
        this.HashTagCounts.Clear();

        this.memoryCache.Dispose();
    }

    public Task<List<TweetDto>> GetAllTweetsAsync(int? page, int? pagesize)
    {
        if (!page.HasValue)
        {
            page = 1;
        }

        if (!pagesize.HasValue || pagesize <= 0)
        {
            pagesize = this.defaultPageSize;
        }

        return Task.FromResult(this.Tweets
            .Values
            .AsEnumerable()
            .Skip(pagesize!.Value * (page.Value - 1))
            .Take(pagesize.Value)
            .ToList());
    }

    public Task<long> GetCountAsync() => Task.FromResult((long)this.Tweets.Count);

    public Task<IEnumerable<KeyValuePair<string, int>>> GetTopHashtags(int top = 10) =>
        Task.FromResult(this
            .HashTagCounts
            .ToList()
            .OrderByDescending(hashTag => hashTag.Value)
            .ThenBy(hashTag => hashTag.Key)
            .Take(top)
            .Select(hashtag =>
                new KeyValuePair<string, int>(hashtag.Key, hashtag.Value)));

    private void InitializePagination(IConfiguration configuration)
    {
        var configurationSection = configuration?.GetSection(ApplicationConfigurationKeys.Pagination);
        var configuratedMaxPageSize = configurationSection?.GetValue<int>(ApplicationConfigurationKeys.MaximumPageSize);

        this.maxPageSize = !configuratedMaxPageSize.HasValue
            || configuratedMaxPageSize <= 0
            ? 1000
            : configuratedMaxPageSize.Value;

        var configuredDefaultPageSize = configurationSection?.GetValue<int>(ApplicationConfigurationKeys.DefaultPageSize);

        this.defaultPageSize = !configuredDefaultPageSize.HasValue
            || configuredDefaultPageSize <= 0
            || configuredDefaultPageSize >= maxPageSize
            ? 50
            : configuredDefaultPageSize.Value;
    }
}