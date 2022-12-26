using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Events.Handlers;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Application.Interfaces.Confluent.Kafka;
using Neal.Twitter.Application.Wrappers.Confluent.Kafka;

namespace Neal.Twitter.Application.UnitTests.TestFixtures;

public class TweetReceivedKafkaHandlerFixture
{
    #region Fields

    private readonly ILoggerFactory _logFactory = LoggerFactory.Create(logger => logger.AddConsole());

    #endregion Fields

    #region Properties

    public IKafkaProducerWrapper MockKafkaProducerWrapper { get; private set; }

    public IMediator MockMediator { get; private set; }

    #endregion Properties

    public TweetReceivedKafkaHandlerFixture()
    {
        // Create verified mediator for DI
        this.MockMediator = Mock.Of<IMediator>(mediator =>
            mediator.Publish(It.IsAny<TweetReceivedNotification>(), It.IsAny<CancellationToken>()) == Task.CompletedTask);

        // Add setup to wrapper for verification - will cause exception when not called by mediator
        var kafkaProducerWrapper = Mock.Of<IKafkaProducerWrapper>(wrapper =>
            wrapper.Produce(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()) == Task.CompletedTask);

        // Build DI container and retrieve wrapper
        this.MockKafkaProducerWrapper = new ServiceCollection()
            .AddSingleton(this._logFactory.CreateLogger<KafkaProducerWrapper>())
            .AddSingleton(this.MockMediator)
            .AddSingleton(kafkaProducerWrapper)
            .AddSingleton<TweetReceivedKafkaHandler>()
            .BuildServiceProvider()
            .GetService<IKafkaProducerWrapper>()
                ?? throw new InvalidOperationException($"Unable to create service collection for {nameof(IKafkaProducerWrapper)}.");
    }
}