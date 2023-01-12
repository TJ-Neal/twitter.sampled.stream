using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Utilities;
using Neal.Twitter.Core.Entities.Configuration;
using Neal.Twitter.Core.Entities.Twitter;
using Neal.Twitter.Faster.Client.Constants;
using Neal.Twitter.Faster.Client.Interfaces;
using System.Net.Http.Json;

namespace Neal.Twitter.Faster.Client.Wrappers;

/// <summary>
/// <inheritdoc cref="IFasterProducerWrapper" />
/// </summary>
public sealed class FasterProducerWrapper : IFasterProducerWrapper
{
    #region Fields

    private readonly FasterConfiguration configuration;

    private readonly ILogger<FasterProducerWrapper> logger;

    private readonly IHttpClientFactory clientFactory;

    private bool hasFaulted = false;

    #endregion Fields

    public FasterProducerWrapper(
        FasterConfiguration fasterConfiguration,
        ILogger<FasterProducerWrapper> logger,
        IHttpClientFactory clientFactory)
    {
        this.configuration = fasterConfiguration;
        this.logger = logger;
        this.clientFactory = clientFactory;
    }

    public async Task ProduceAsync(TweetDto message, CancellationToken cancellationToken)
    {
        // TODO: Create a timer for resetting faulted so producers can retry after t time
        if (!this.configuration.Enabled
            || hasFaulted
            || cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (string.IsNullOrEmpty(this.configuration.BaseUrl))
        {
            throw new InvalidOperationException(CommonLogMessages.InvalidConfiguration);
        }

        int retries = 0;

        while (retries <= RetryUtilities.MaxRetries)
        {
            try
            {
                using var client = clientFactory.CreateClient();

                var result = await client.PostAsJsonAsync(this.configuration.BaseUrl, new List<TweetDto> { message }, cancellationToken);

                if (result is null || !result.IsSuccessStatusCode)
                {
                    this.logger.LogError(HandlerLogMessages.ProducerError, message.Id, message);
                }
                else
                {
                    this.logger.LogDebug(HandlerLogMessages.PrintResult, DateTime.Now, result.StatusCode, message.Id);

                    return;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(HandlerLogMessages.ProducerException, ex.Message);

                // Exit loop if exception is not transient
                if (!RetryUtilities.ExceptionIsTransient(ex))
                {
                    this.hasFaulted = true;

                    return;
                }
            }

            retries++;
            await Task.Delay(RetryUtilities.GetDelay(retries), cancellationToken);
        }

        this.hasFaulted = true;
    }
}