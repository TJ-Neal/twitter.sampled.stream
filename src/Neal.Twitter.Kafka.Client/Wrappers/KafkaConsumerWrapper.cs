using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Interfaces.TweetRepository;
using Neal.Twitter.Core.Entities.Twitter;
using Neal.Twitter.Kafka.Client.Constants;
using Neal.Twitter.Kafka.Client.Interfaces;
using Serilog;
using System.Text.Json;

namespace Neal.Twitter.Kafka.Client.Wrappers;

public class KafkaConsumerWrapper : IKafkaConsumerWrapper<string, string>
{
    private readonly IConsumer<string, string> consumer;

    private readonly ILogger<KafkaConsumerWrapper> logger;

    private readonly ITweetRepository tweetRepository;

    public KafkaConsumerWrapper(IConfiguration configuration, ILogger<KafkaConsumerWrapper> logger, ITweetRepository tweetRepository)
    {
        this.logger = logger;
        this.tweetRepository = tweetRepository;
        var kafkaConfig = configuration
            .GetSection(nameof(ConsumerConfig))
            .Get<ConsumerConfig>();

        this.consumer = new ConsumerBuilder<string, string>(kafkaConfig)
            .Build();
    }

    public async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        this.consumer.Subscribe("twitter-V2-sampled-stream");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = await Task.Run(() => this.consumer.Consume(cancellationToken));

                var tweet = JsonSerializer.Deserialize<TweetDto>(result.Message.Value);

                if (tweet is not null)
                {
                    await this.tweetRepository.AddRecordsAsync(new List<TweetDto> { tweet });

                    Log.Information(
                        HandlerLogMessages.PrintConsumerResult,
                        DateTime.Now.ToUniversalTime,
                        result.Message.Key,
                        result.Message.Timestamp.UtcDateTime);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        this.consumer.Unsubscribe();
    }

    public void Dispose()
    {
        try
        {
            if (this.consumer is not null)
            {
                this.consumer.Unsubscribe();
                this.consumer.Close();
                this.consumer.Dispose();
            }
        }
        catch (Exception ex)
        {
            this.logger
                .LogError(ExceptionMessages.DisposeException, nameof(this.consumer), ex.Message);
        }

        GC.SuppressFinalize(this);
    }
}