using FASTER.core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants.Keys;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Interfaces.TweetRepository;
using Neal.Twitter.Core.Entities.Twitter;
using System.Text.Json;

namespace Neal.Twitter.Infrastructure.Faster.Repository.Services.Repository;

public sealed class FasterTweetRepository : ITweetRepository
{
    // Justification - Initializers used in constructor
#pragma warning disable CS8618

    #region Fields

    #region Tweet Store

    private static FasterKV<string, string> tweetsStore;

    private static AsyncPool<ClientSession<string, string, string, string, Empty, IFunctions<string, string, string, string, Empty>>> tweetsSessionPool;

    private static IDevice tweetsObjectLog;

    private static IDevice tweetsLog;

    #endregion Tweet Store

    #region Hashtags Store

    private static FasterKV<string, int> hashtagsStore;

    private static AsyncPool<ClientSession<string, int, int, int, Empty, IFunctions<string, int, int, int, Empty>>> hashtagsSessionPool;

    private static IDevice hashtagsObjectLog;

    private static IDevice hashtagsLog;

    private static readonly CancellationTokenSource cancellationTokenSource = new();

    #endregion Hashtags Store

    private static Thread commitThread;

    private readonly ILogger<FasterTweetRepository> logger;

    private int defaultPagesize;

    private int maxPageSize;

    #endregion Fields

#pragma warning restore CS8618

    public FasterTweetRepository(ILogger<FasterTweetRepository> logger, IConfiguration configuration)
    {
        // TODO: Create a FasterKV configuration model so this can be configured to be project specific
        string rootPath = Path.Combine(Path.GetTempPath(), "Logs", "Faster");
        this.logger = logger;

        // TODO: Create object for this that .Get<Pagination> can handle.
        this.InitializePagination(configuration);

        InstantiateTweetsStore(rootPath);
        InstantiateHashtagsStore(rootPath);
    }

    public void Dispose()
    {
        // Cancel background commit thread
        cancellationTokenSource.Cancel();

        // Dispose of pools and stores - ensures graceful shutdown
        hashtagsSessionPool.Dispose();
        hashtagsStore.Dispose();
        tweetsSessionPool.Dispose();
        tweetsStore.Dispose();
    }

    public async Task AddRecordsAsync(IEnumerable<TweetDto> tweets)
    {
        var tweetsSession = GetTweetsSession();
        var hashtagsSession = GetHashtagsSession();

        try
        {
            foreach (var tweet in tweets)
            {
                await tweetsSession.RMWAsync(new string(tweet.Id), JsonSerializer.Serialize(tweet));

                if (tweet.Hashtags is null
                    || !tweet.Hashtags.Any())
                {
                    continue;
                }

                tweet.Hashtags
                    .Where(hashtag => !string.IsNullOrWhiteSpace(hashtag.Tag))
                    .Select(hashtag => hashtag.Tag)
                    .ToList()
                    .ForEach(async hashtag => await hashtagsSession.RMWAsync(hashtag!, 1));
            }

            // Ensure a commit thread is running once tweets have been added at least once
            if (commitThread is null)
            {
                commitThread = new Thread(new ThreadStart(() => CommitThread(cancellationTokenSource.Token)));
                commitThread.Start();
            }
        }
        catch (Exception ex)
        {
            this.logger.LogCritical(ExceptionMessages.GenericException, ex.Message);
        }

        tweetsSessionPool.Return(tweetsSession);
        hashtagsSessionPool.Return(hashtagsSession);
    }

    public Task<List<TweetDto>> GetAllTweetsAsync(int? page, int? pagesize)
    {
        int currentPage = page ?? 1;
        int currentPagesize = pagesize ?? this.defaultPagesize;
        int recordSize = tweetsStore.Log.FixedRecordSize;
        long headAddress = tweetsStore.Log.HeadAddress;
        long pageBytes = currentPagesize * recordSize;
        long pageStart = headAddress + ((currentPage - 1) * pageBytes);
        long pageEnd = pageStart + pageBytes;
        var tweetsSession = GetTweetsSession();
        var output = new List<TweetDto>();

        var iterator = tweetsSession.Iterate(pageEnd);

        while (iterator.GetNext(out var recordInfo, out string key, out string value))
        {
            if (iterator.CurrentAddress < pageStart)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    var dto = JsonSerializer.Deserialize<TweetDto>(value);

                    if (dto is not null)
                    {
                        output.Add(dto);
                    }
                }
                catch (Exception)
                {
                    this.logger.LogCritical(ExceptionMessages.SerializationError, nameof(TweetDto));
                }
            }
        }

        tweetsSessionPool.Return(tweetsSession);

        return Task.FromResult(output);
    }

    public Task<long> GetCountAsync()
    {
        long logCount = (tweetsStore.Log.TailAddress - tweetsStore.Log.HeadAddress) / tweetsStore.Log.FixedRecordSize;
        long memoryCount = (tweetsStore.ReadCache.TailAddress - tweetsStore.ReadCache.HeadAddress) / tweetsStore.ReadCache.FixedRecordSize;

        return Task.FromResult(logCount + memoryCount);
    }

    public Task<IEnumerable<KeyValuePair<string, int>>> GetTopHashtags(int top = 10)
    {
        var hashtagsSession = GetHashtagsSession();
        var hashtags = new Dictionary<string, int>();
        var iterator = hashtagsSession.Iterate();

        while (iterator.GetNext(out var recordInfo, out string key, out int value))
        {
            if (!string.IsNullOrEmpty(key)
                && value > 0)
            {
                hashtags.Add(key, value);
            }
        }

        var results = hashtags
            .ToList()
            .OrderByDescending(hashtag => hashtag.Value)
            .ThenBy(hashtag => hashtag.Key)
            .Take(top)
            .Select(hashtag => new KeyValuePair<string, int>(hashtag.Key, hashtag.Value));

        hashtagsSessionPool.Return(hashtagsSession);

        return Task.FromResult(results);
    }

    #region Private Methods

    private static void CommitThread(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Thread.Sleep(1000);

            tweetsStore
                .TakeHybridLogCheckpointAsync(CheckpointType.Snapshot)
                .AsTask()
                .GetAwaiter()
                .GetResult();
            hashtagsStore
                .TakeHybridLogCheckpointAsync(CheckpointType.Snapshot)
                .AsTask()
                .GetAwaiter()
                .GetResult();
        }
    }

    #region Initialization

    private static void InstantiateTweetsStore(string rootPath)
    {
        string tweetsPath = Path.Combine(rootPath, "tweets");
        var tweetFuncs = new SimpleFunctions<string, string>((a, b) => b);

        tweetsObjectLog = Devices.CreateLogDevice(Path.Combine(tweetsPath, "_obj.log"), recoverDevice: true);
        tweetsLog = Devices.CreateLogDevice(Path.Combine(tweetsPath, ".log"), recoverDevice: true);

        var settings = new FasterKVSettings<string, string>(tweetsPath, deleteDirOnDispose: false)
        {
            IndexSize = 1L << 20,
            TryRecoverLatest = true,
            LogDevice = tweetsLog,
            ObjectLogDevice = tweetsObjectLog,
            CheckpointDir = Path.Combine(tweetsPath, "CheckPoints"),
            ReadCacheEnabled = true
        };

        tweetsStore = new FasterKV<string, string>(settings);
        tweetsSessionPool = new AsyncPool<ClientSession<string, string, string, string, Empty, IFunctions<string, string, string, string, Empty>>>(
            settings.LogDevice.ThrottleLimit,
            () => tweetsStore.For(tweetFuncs).NewSession<IFunctions<string, string, string, string, Empty>>());
    }

    private static void InstantiateHashtagsStore(string rootPath)
    {
        string hashtagsPath = Path.Combine(rootPath, "hashtags");
        var hashtagsFuncs = new SimpleFunctions<string, int>((a, b) => a + b);

        hashtagsObjectLog = Devices.CreateLogDevice(Path.Combine(hashtagsPath, "_obj.log"), recoverDevice: true);
        hashtagsLog = Devices.CreateLogDevice(Path.Combine(hashtagsPath, ".log"), recoverDevice: true);

        var settings = new FasterKVSettings<string, int>(hashtagsPath, deleteDirOnDispose: false)
        {
            IndexSize = 1L << 20,
            TryRecoverLatest = true,
            LogDevice = hashtagsObjectLog,
            ObjectLogDevice = hashtagsLog,
            CheckpointDir = Path.Combine(hashtagsPath, "CheckPoints")
        };

        hashtagsStore = new FasterKV<string, int>(settings);
        hashtagsSessionPool = new AsyncPool<ClientSession<string, int, int, int, Empty, IFunctions<string, int, int, int, Empty>>>(
            settings.LogDevice.ThrottleLimit,
            () => hashtagsStore.For(hashtagsFuncs).NewSession<IFunctions<string, int, int, int, Empty>>());
    }

    private void InitializePagination(IConfiguration configuration)
    {
        var configurationSection = configuration?.GetSection(ApplicationConfigurationKeys.Pagination);
        int? configuratedMaxPageSize = configurationSection?
            .GetValue<int>(ApplicationConfigurationKeys.MaximumPageSize);

        this.maxPageSize = !configuratedMaxPageSize.HasValue
            || configuratedMaxPageSize <= 0
                ? 1000
                : configuratedMaxPageSize.Value;

        int? configuredDefaultPageSize = configurationSection?
            .GetValue<int>(ApplicationConfigurationKeys.DefaultPageSize);

        this.defaultPagesize = !configuredDefaultPageSize.HasValue
            || configuredDefaultPageSize <= 0
            || configuredDefaultPageSize >= maxPageSize
                ? 50
                : configuredDefaultPageSize.Value;
    }

    #endregion Initialization

    #region Sessions

    private static ClientSession<string, int, int, int, Empty, IFunctions<string, int, int, int, Empty>> GetHashtagsSession()
    {
        if (!hashtagsSessionPool.TryGet(out var hashtagsSession))
        {
            hashtagsSession = hashtagsSessionPool
                .GetAsync()
                .AsTask()
                .GetAwaiter()
                .GetResult();
        }

        return hashtagsSession;
    }

    private static ClientSession<string, string, string, string, Empty, IFunctions<string, string, string, string, Empty>> GetTweetsSession()
    {
        if (!tweetsSessionPool.TryGet(out var tweetsSession))
        {
            tweetsSession = tweetsSessionPool
                .GetAsync()
                .AsTask()
                .GetAwaiter()
                .GetResult();
        }

        return tweetsSession;
    }

    #endregion Sessions

    #endregion Private Methods
}