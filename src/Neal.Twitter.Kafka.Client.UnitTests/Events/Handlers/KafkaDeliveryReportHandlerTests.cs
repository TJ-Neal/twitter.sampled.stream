using Confluent.Kafka;
using Neal.Twitter.Kafka.Client.Events.Notifications;
using Neal.Twitter.Kafka.Client.UnitTests.TestFixtures;

namespace Neal.Twitter.Kafka.Client.UnitTests.Events.Handlers;

public class KafkaDeliveryReportHandlerTests : IClassFixture<DeliveryReportHandlerFixture>
{
    #region Constants

    private const string TEST_EXCEPTION_MESSAGE = "Test error";

    #endregion Constants

    #region Fields

    private readonly DeliveryReportHandlerFixture kafkaDeliveryReportHandlerFixture;

    private readonly DeliveryReport<string, string> deliveryReportWithError;

    public KafkaDeliveryReportHandlerTests(DeliveryReportHandlerFixture kafkaDeliveryReportHandlerFixture)
    {
        this.kafkaDeliveryReportHandlerFixture = kafkaDeliveryReportHandlerFixture;

        var error = new Error(ErrorCode.OperationNotAttempted, TEST_EXCEPTION_MESSAGE);

        this.deliveryReportWithError = new DeliveryReport<string, string>
        {
            Message = new Message<string, string>
            {
                Key = "Test Key",
                Value = "Test Value"
            },
            Error = error
        };
    }

    #endregion Fields

    [Fact]
    public async Task KafkaDeliveryReportHandler_WithTokenCancelled_WithError_DoesNotExcept_Handle()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        // Arrange
        var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;

        // Act
        cancellationSource.Cancel();

        var task = this.kafkaDeliveryReportHandlerFixture
            .DeliveryReportHandler
            .Handle(
                new DeliveryReportNotification<string, string>(this.deliveryReportWithError, cancellationToken),
                cancellationToken);

        // Assert
        Assert.True(task == Task.CompletedTask);
    }

    [Fact]
    public async Task KafkaDeliveryReportHandler_WithoutTokenCancelled_WithError_DoesNotExcept_Handle()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        // Arrange

        var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;

        // Act
        var task = this.kafkaDeliveryReportHandlerFixture
            .DeliveryReportHandler
            .Handle(
                new DeliveryReportNotification<string, string>(this.deliveryReportWithError, cancellationToken),
                cancellationToken);

        // Assert
        Assert.True(task == Task.CompletedTask);
    }
}