using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Neal.Twitter.Infrastructure.StreamReader.Services.TwitterApi.V2;

namespace Neal.Twitter.Infrastructure.StreamReader.Tests;

public class TwitterStreamReaderServiceTests
{
    #region Service Under Test

    private IHostedService? _service;

    #endregion

    #region Services for injection

    private Mock<ILogger<TwitterStreamReaderService>> _mockLogger;
    private IConfiguration _configuration;
    private Mock<IMediator> _mockMediator;

    #endregion

    public void SetUp()
    {
        const string tweetText = "This is a test tweet";
        var hashtag = new TweetEntityHashtag
        {
            Start = 10,
            End = 20,
            Tag = "TEST"
        };
        var mockTweet = new Tweet
        {
            Text = tweetText,
            Entities = new TweetEntities
            {
                Hashtags = new List<TweetEntityHashtag> { hashtag }
            }
        };

        this._configuration = new ConfigurationBuilder().Build();
        this._mockLogger = new Mock<ILogger<TwitterStreamReaderService>>();
        this._mockMediator = new Mock<IMediator>();
        this._mockAuthorizer = new Mock<IAuthorizer>();

        this._mockAuthorizer
            .Setup(authorizer => authorizer.AuthorizeAsync())
            .Returns(Task.FromResult(true));

        this._mockTwitterContext = new Mock<TwitterContext>(this._mockAuthorizer.Object);
        this._mockStreaming = new Mock<Streaming>();
        
        var serializedTweet = JsonSerializer.Serialize(mockTweet);
        var mockTwitterProcessor = new Mock<ITwitterExecute>();

        mockTwitterProcessor
            .Setup(twitterExecute => twitterExecute.QueryTwitterStreamAsync(
                It.Is<Request>(request => request.Endpoint == "testEndPoint")))
            .ReturnsAsync("");

        var streamContent = new LinqToTwitter.StreamContent(
            mockTwitterProcessor.Object,
            serializedTweet);

        this._mockStreaming
            .SetReturnsDefault(streamContent);

        var services = new ServiceCollection();

        var serviceProvider = services.AddSingleton<IHostedService, TwitterStreamReaderService>()
            .AddSingleton(this._mockLogger.Object)
            .AddSingleton(this._configuration)
            .AddSingleton(this._mockTwitterContext.Object)
            .AddSingleton(this._mockMediator.Object)
            .BuildServiceProvider();

        this._service = serviceProvider.GetService<IHostedService>();

        if (this._service is null)
        {
            throw new InvalidOperationException("Unable to create hosted service");
        }
    }

    [Fact]
    public async Task StartAsync_WithCancelledTask_DoesNotStartStreaming_Success()
    {
        // Arrange
        var cancellationSource = new CancellationTokenSource();
        var stoppingToken = cancellationSource.Token;
        var task = this._service!.StartAsync(stoppingToken);

        // Act
        cancellationSource.Cancel(); // Cancel before task is started       
        task.Wait();

        // Assert
        Assert.True(task.IsCanceled);
    }

    [Fact]
    public async Task ExecuteAsync_Returns_Stream_Content_Success()
    {
        // Arrange
        
        var cancellationSource = new CancellationTokenSource();
        var stoppingToken = cancellationSource.Token;

        // Act
        try
        {
            // Change to test if log message is as expected
            if (this._service is not null)
            {
                cancellationSource.CancelAfter(TimeSpan.FromSeconds(15));
                await this._service.StartAsync(stoppingToken);
            }
            else
            {
                throw new InvalidOperationException("Unable to create hosted service");
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !stoppingToken.IsCancellationRequested)
        {
            Assert.Fail($"Exception of thrown:\n{ex}");
        }

        // Assert
        // Passes
    }
}