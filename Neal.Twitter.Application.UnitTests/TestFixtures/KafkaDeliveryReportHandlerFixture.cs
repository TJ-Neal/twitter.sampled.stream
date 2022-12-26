using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Events.Handlers;
using Neal.Twitter.Application.Events.Notifications;

namespace Neal.Twitter.Application.UnitTests.TestFixtures;

public class KafkaDeliveryReportHandlerFixture
{
    #region Fields

    private readonly ILoggerFactory _logFactory = LoggerFactory.Create(logger => logger.AddConsole());

    #endregion Fields

    #region Properties

    public KafkaDeliveryReportHandler KafkaDeliveryReportHandler { get; private set; }

    #endregion Properties

    public KafkaDeliveryReportHandlerFixture()
    {
        this.KafkaDeliveryReportHandler = new ServiceCollection()
            .AddSingleton(this._logFactory.CreateLogger<KafkaDeliveryReportHandler>())
            .AddSingleton<KafkaDeliveryReportHandler>()
            .BuildServiceProvider()
            .GetRequiredService<KafkaDeliveryReportHandler>()
                ?? throw new InvalidOperationException($"Unable to create service collection for {nameof(KafkaDeliveryReportHandler)}.");
    }
}