using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Core.Entities.Twitter;
using Neal.Twitter.Infrastructure.Simple.Repository.Services.Repository;

namespace Neal.Twitter.Infrastructure.Simple.Repository.UnitTests.Services;

public class TweetRepositoryTests
{
    #region Fields

    private readonly ILoggerFactory logFactory = LoggerFactory.Create(logger => logger.AddConsole());

    private readonly TweetRepository tweetRepository;

    #endregion Fields

    public TweetRepositoryTests() => this.tweetRepository = new ServiceCollection()
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
            .AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()))
            .AddSingleton(this.logFactory.CreateLogger<TweetRepository>())
            .AddSingleton(typeof(TweetRepository))
            .BuildServiceProvider()
            .GetService<TweetRepository>()
                ?? throw new InvalidOperationException("Unable to create Tweet Repository.");

    [Fact]
    public async Task TweetRepository_SingleTweet_NoHashtags_WriteAndRead_Success()
    {
        // Arrange
        var service = this.tweetRepository;

        // Act
        await service.AddRecordsAsync(new List<TweetDto> { new TweetDto("123", "This is a tweet without hashtags", null) });

        int afterCount = (await service.GetAllTweetsAsync(1, 1)).Count;
        int afterHashtagsCount = (await service.GetTopHashtags(10)).Count();

        // Assert
        Assert.NotNull(service);
        Assert.True(afterCount == 1);
        Assert.True(afterHashtagsCount == 0);
    }

    [Fact]
    public async Task TweetRepository_MultipleTweets_WithHashtags_WriteAndRead_Success()
    {
        // Arrange
        var service = this.tweetRepository;
        int numberOfTweets = 100;
        var hashtags = new Hashtag[3] { new(1, 5, "TEST"), new(6, 7, "TEST2"), new(3, 10, "TEST3") };

        // Act
        for (int i = 1; i <= numberOfTweets; i++)
        {
            await service.AddRecordsAsync(new List<TweetDto> { new TweetDto(i.ToString(), i.ToString(), hashtags) });
        }

        int afterCount = (await service.GetAllTweetsAsync(1, numberOfTweets)).Count;
        long totalCount = await service.GetCountAsync();
        int afterHashtagsCount = (await service.GetTopHashtags(10)).Count();

        // Assert
        Assert.NotNull(service);
        Assert.True(afterCount == numberOfTweets);
        Assert.True(totalCount == numberOfTweets);
        Assert.True(afterHashtagsCount == hashtags.Length);
    }
}