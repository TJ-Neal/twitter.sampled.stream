using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Client.Kafka.Interfaces;

namespace Neal.Twitter.Infrastructure.KafkaReader.Services.Kafka;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> logger;

    private readonly IKafkaConsumerWrapper<string, string> kafkaConsumerWrapper;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IKafkaConsumerWrapper<string, string> kafkaConsumerWrapper)
    {
        this.logger = logger;
        this.kafkaConsumerWrapper = kafkaConsumerWrapper;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation(CommonLogMessages.StartingLoop);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await this.kafkaConsumerWrapper.ConsumeAsync(cancellationToken);
            }

            this.logger.LogInformation(CommonLogMessages.CancelRequested);
        }
        catch (Exception ex)
        {
            this.logger.LogCritical(ExceptionMessages.ErrorDuringLoop, ex);
        }
    }
}