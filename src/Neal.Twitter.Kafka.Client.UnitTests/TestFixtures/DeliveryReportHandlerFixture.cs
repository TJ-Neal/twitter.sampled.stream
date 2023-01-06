using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Kafka.Client.Events.Handlers;

namespace Neal.Twitter.Kafka.Client.UnitTests.TestFixtures;

public class DeliveryReportHandlerFixture
{
    #region Fields

    private readonly ILoggerFactory logFactory = LoggerFactory.Create(logger => logger.AddConsole());

    #endregion Fields

    #region Properties

    public DeliveryReportHandler DeliveryReportHandler { get; private set; }

    #endregion Properties

    public DeliveryReportHandlerFixture()
    {
        this.DeliveryReportHandler = new ServiceCollection()
            .AddSingleton(this.logFactory.CreateLogger<DeliveryReportHandler>())
            .AddSingleton<DeliveryReportHandler>()
            .BuildServiceProvider()
            .GetRequiredService<DeliveryReportHandler>()
                ?? throw new InvalidOperationException($"Unable to create service collection for {nameof(DeliveryReportHandler)}.");
    }
}