using Confluent.Kafka;
using MediatR;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Application.UnitTests.TestFixtures;

namespace Neal.Twitter.Application.UnitTests.Events.Handlers;

public class KafkaDeliveryReportHandlerTests : IClassFixture<KafkaDeliveryReportHandlerFixture>
{
    #region Constants

    private const string TEST_EXCEPTION_MESSAGE = "Test error";

    #endregion

    #region Fields

    private readonly KafkaDeliveryReportHandlerFixture _kafkaDeliveryReportHandlerFixture;

    private readonly DeliveryReport<string, string> _deliveryReportWithError;

    public KafkaDeliveryReportHandlerTests(KafkaDeliveryReportHandlerFixture kafkaDeliveryReportHandlerFixture)
    {
        this._kafkaDeliveryReportHandlerFixture = kafkaDeliveryReportHandlerFixture;

        var error = new Error(ErrorCode.OperationNotAttempted, TEST_EXCEPTION_MESSAGE);

        this._deliveryReportWithError = new DeliveryReport<string, string>
        {
            Message = new Message<string, string>
            {
                Key = "Test Key",
                Value = "Test Value"
            },
            Error = error
        };
    } 
        

    #endregion

    [Fact]
    public async Task KafkaDeliveryReportHandler_WithTokenCancelled_WithError_DoesNotExcept_Handle()
    {
        // Arrange
        var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;

        // Act
        cancellationSource.Cancel();

        var task = this._kafkaDeliveryReportHandlerFixture
            .KafkaDeliveryReportHandler
            .Handle(
                new KafkaDeliveryResultNotification(this._deliveryReportWithError, cancellationToken),
                cancellationToken);

        // Assert
        Assert.True(task == Task.CompletedTask);
    }

    [Fact]
    public async Task KafkaDeliveryReportHandler_WithoutTokenCancelled_WithError_DoesNotExcept_Handle()
    {
        // Arrange
        
        var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;

        // Act
        var task = this._kafkaDeliveryReportHandlerFixture
            .KafkaDeliveryReportHandler
            .Handle(
                new KafkaDeliveryResultNotification(this._deliveryReportWithError, cancellationToken), 
                cancellationToken);

        // Assert
        Assert.True(task == Task.CompletedTask);
    }
}