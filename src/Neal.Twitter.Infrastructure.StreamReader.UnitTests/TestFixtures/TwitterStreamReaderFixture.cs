using LinqToTwitter.OAuth;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Core.Entities.Twitter;
using Neal.Twitter.Infrastructure.StreamReader.Services.TwitterApi.V2;
using Neal.Twitter.LinqToTwitter.Client.Interfaces;
using Neal.Twitter.LinqToTwitter.Client.Wrappers;
using StreamContent = LinqToTwitter.StreamContent;

namespace Neal.Twitter.Infrastructure.StreamReader.UnitTests.TestFixtures;

public class TwitterStreamReaderFixture
{
    #region Fields

    private readonly ILoggerFactory logFactory = LoggerFactory.Create(logger => logger.AddConsole());

    private readonly IMediator mockMediator = Mock.Of<IMediator>(mediator =>
        mediator.Publish(new TweetReceivedNotification(new TweetDto("123", string.Empty, Array.Empty<Hashtag>())),
            default) == Task.CompletedTask);

    #endregion Fields

    #region Properties

    public IHostedService HostedService { get; private set; }

    public ILinqToTwitterWrapper MockLinqToTwitterWrapper { get; private set; }

    #endregion Properties

    // Setup Test fixture for all Twitter Stream Reader Tests
    public TwitterStreamReaderFixture()
    {
        var mockAuthorizerWrapper = new Mock<ApplicationOnlyAuthorizerWrapper>(Mock.Of<IAuthorizer>(authorizer =>
            authorizer.AuthorizeAsync() == Task.FromResult(true)));

        this.MockLinqToTwitterWrapper = Mock.Of<ILinqToTwitterWrapper>(wrapper =>
            wrapper.StartAsync(It.IsAny<Func<StreamContent, Task>>(), It.IsAny<CancellationToken>()) == Task.CompletedTask);
        this.HostedService = new ServiceCollection()
            .AddSingleton(new ConfigurationBuilder().Build() as IConfiguration)
            .AddSingleton(this.logFactory.CreateLogger<LinqToTwitterWrapper>())
            .AddSingleton(this.logFactory.CreateLogger<TwitterStreamReaderService>())
            .AddSingleton(this.mockMediator)
            .AddSingleton(mockAuthorizerWrapper.Object)
            .AddSingleton(this.MockLinqToTwitterWrapper)
            .AddSingleton<IHostedService, TwitterStreamReaderService>()
            .BuildServiceProvider()
            .GetService<IHostedService>()
                ?? throw new InvalidOperationException("Unable to create hosted service.");
    }
}