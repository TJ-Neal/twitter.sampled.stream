using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Application.Interfaces;
using Neal.Twitter.Core.Entities.Twitter;
using Neal.Twitter.Kafka.Client.Events.Handlers;
using Neal.Twitter.Kafka.Client.Wrappers;

namespace Neal.Twitter.Kafka.Client.UnitTests.TestFixtures;

public class TweetReceivedKafkaHandlerFixture
{
    #region Fields

    private readonly ILoggerFactory logFactory = LoggerFactory.Create(logger => logger.AddConsole());

    #endregion Fields

    #region Properties

    public IProducerWrapper<TweetDto> MockKafkaProducerWrapper { get; private set; }

    public IMediator MockMediator { get; private set; }

    #endregion Properties

    public TweetReceivedKafkaHandlerFixture()
    {
        // Create verified mediator for DI
        this.MockMediator = Mock.Of<IMediator>(mediator =>
            mediator.Publish(It.IsAny<TweetReceivedNotification>(), It.IsAny<CancellationToken>()) == Task.CompletedTask);

        // Add setup to wrapper for verification - will cause exception when not called by mediator
        var kafkaProducerWrapper = Mock.Of<IProducerWrapper<TweetDto>>(wrapper =>
            wrapper.ProduceAsync(It.IsAny<TweetDto>(), It.IsAny<CancellationToken>()) == Task.CompletedTask);

        // Build DI container and retrieve wrapper
        this.MockKafkaProducerWrapper = new ServiceCollection()
            .AddSingleton(this.logFactory.CreateLogger<KafkaProducerWrapper>())
            .AddSingleton(this.MockMediator)
            .AddSingleton(kafkaProducerWrapper)
            .AddSingleton<KafkaTweetReceivedHandler>()
            .BuildServiceProvider()
            .GetService<IProducerWrapper<TweetDto>>()
                ?? throw new InvalidOperationException($"Unable to create service collection for {nameof(KafkaProducerWrapper)}.");
    }
}