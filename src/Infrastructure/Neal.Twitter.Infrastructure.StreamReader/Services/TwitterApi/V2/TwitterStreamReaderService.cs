using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.LinqToTwitter.Client.Interfaces;

namespace Neal.Twitter.Infrastructure.StreamReader.Services.TwitterApi.V2;

/// <summary>
/// Service to marshal requests for the Twitter Volume Sample Stream
/// </summary>
public class TwitterStreamReaderService : BackgroundService
{
    private readonly ILogger<TwitterStreamReaderService> logger;

    private readonly ILinqToTwitterWrapper linqToTwitterWrapper;

    public TwitterStreamReaderService(
        ILogger<TwitterStreamReaderService> logger,
        ILinqToTwitterWrapper linqToTwitterWrapper)
    {
        this.logger = logger;
        this.linqToTwitterWrapper = linqToTwitterWrapper;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation(CommonLogMessages.StartingLoop);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await this.linqToTwitterWrapper
                    .StartAsync(
                        async streamContent =>
                            await this.linqToTwitterWrapper.ProcessStreamContent(streamContent, cancellationToken),
                        cancellationToken);
            }

            this.logger.LogInformation(CommonLogMessages.CancelRequested);
        }
        catch (Exception ex)
        {
            this.logger.LogCritical(ExceptionMessages.ErrorDuringLoop, ex);
        }
    }
}