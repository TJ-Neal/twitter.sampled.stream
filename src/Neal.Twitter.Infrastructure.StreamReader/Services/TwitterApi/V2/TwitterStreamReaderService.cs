using Neal.Twitter.Application.Constants;
using Neal.Twitter.Application.Constants.Configuration;
using Neal.Twitter.Application.Interfaces.LinqToTwitter;

namespace Neal.Twitter.Infrastructure.StreamReader.Services.TwitterApi.V2;

/// <summary>
/// Service to marshal requests for the Twitter Volume Sample Stream
/// </summary>
public class TwitterStreamReaderService : BackgroundService
{
    #region Fields

    private readonly ILogger<TwitterStreamReaderService> _logger;

    private readonly ILinqToTwitterWrapper _linqToTwitterWrapper;

    private readonly string _topic;

    #endregion Fields

    public TwitterStreamReaderService(
        ILogger<TwitterStreamReaderService> logger,
        IConfiguration configuration,
        ILinqToTwitterWrapper linqToTwitterWrapper)
    {
        this._logger = logger;
        this._linqToTwitterWrapper = linqToTwitterWrapper;
        this._topic = configuration.GetValue<string>(SectionKeys.Topic) ?? string.Empty;
    }

    #region Overrides

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Loop until cancellation is requested
        while (!cancellationToken.IsCancellationRequested)
        {
            await this._linqToTwitterWrapper
                .StartAsync(
                    async streamContent =>
                        await this._linqToTwitterWrapper.ProcessStreamContent(streamContent, this._topic, cancellationToken),
                    cancellationToken);
        }

        // Cancellation requested, stream ended
        this._logger.LogInformation(CommonLogMessages.CancelRequested);
    }

    #endregion Overrides
}