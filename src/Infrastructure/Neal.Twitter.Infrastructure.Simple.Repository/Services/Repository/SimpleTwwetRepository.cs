using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants.Keys;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Interfaces.TweetRepository;
using Neal.Twitter.Core.Entities.Configuration;
using Neal.Twitter.Core.Entities.Twitter;
using System.Collections.Concurrent;

namespace Neal.Twitter.Infrastructure.Simple.Repository.Services.Repository;

public sealed class SimpleTweetRepository : ITweetRepository
{
    #region Fields

    private readonly IMemoryCache memoryCache;

    private readonly ILogger<SimpleTweetRepository> logger;

    private readonly Thread heartbeatThread;

    private readonly CancellationTokenSource cancellationTokenSource;

    #endregion Fields

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

    public SimpleTweetRepository(IMemoryCache memoryCache, ILogger<SimpleTweetRepository> logger)
    {
        this.memoryCache = memoryCache;
        this.logger = logger;
        this.cancellationTokenSource = new CancellationTokenSource();

        // Ensure a commit thread is running once tweets have been added at least once
        if (this.heartbeatThread is null)
        {
            this.heartbeatThread = new Thread(new ThreadStart(() => this.HeartbeatThread(this.cancellationTokenSource.Token)));
            this.heartbeatThread.Start();
        }
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
        this.logger.LogInformation(CommonLogMessages.Disposing, nameof(SimpleTweetRepository));

        this.cancellationTokenSource.Cancel();
        this.Tweets.Clear();
        this.HashTagCounts.Clear();

        this.memoryCache.Dispose();
    }

    public Task<List<TweetDto>> GetAllTweetsAsync(Pagination pagination) =>
        Task.FromResult(this.Tweets
            .Values
            .AsEnumerable()
            .Skip(pagination.PageSize * (pagination.Page - 1))
            .Take(pagination.PageSize)
            .ToList());

    public Task<long> GetCountAsync() => Task.FromResult((long)this.Tweets.Count);

    public Task<IEnumerable<KeyValuePair<string, int>>> GetTopHashtags(int top = 10)
    {
        var hashtags = this
            .HashTagCounts
            .ToList()
            .OrderByDescending(hashTag => hashTag.Value)
            .ThenBy(hashTag => hashTag.Key)
            .Take(top);

        return Task.FromResult(hashtags);
    }

    private void HeartbeatThread(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Thread.Sleep(TimeSpan.FromSeconds(30));

            this.logger.LogInformation(
                ApplicationStatusMessages.TweetCount,
                nameof(SimpleTweetRepository),
                this.Tweets.Count,
                this.HashTagCounts.Count);
        }
    }
}