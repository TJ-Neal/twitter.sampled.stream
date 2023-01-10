using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Utilities;
using Neal.Twitter.Core.Entities.Configuration;
using Neal.Twitter.Core.Entities.Twitter;
using Neal.Twitter.Simple.Client.Constants;
using Neal.Twitter.Simple.Client.Interfaces;
using System.Net.Http.Json;

namespace Neal.Twitter.Simple.Client.Wrappers;

/// <summary>
/// <inheritdoc cref="ISimplerProducerWrapper" />
/// </summary>
public sealed class SimpleProducerWrapper : ISimpleProducerWrapper
{
    #region Fields

    private readonly SimpleConfiguration configuration;

    private readonly ILogger<SimpleProducerWrapper> logger;

    private readonly IHttpClientFactory clientFactory;

    private bool hasFaulted = false;

    #endregion Fields

    public SimpleProducerWrapper(
        ILogger<SimpleProducerWrapper> logger,
        IHttpClientFactory clientFactory,
        SimpleConfiguration simpleConfiguration)
    {
        this.configuration = simpleConfiguration;
        this.logger = logger;
        this.clientFactory = clientFactory;
    }

    public async Task ProduceAsync(TweetDto message, CancellationToken cancellationToken)
    {
        // TODO: Create a timer for resetting faulted so producers can retry after t time
        if (!this.configuration.Enabled
            || this.hasFaulted
            || cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (string.IsNullOrEmpty(this.configuration.BaseUrl))
        {
            throw new InvalidOperationException(CommonLogMessages.InvalidConfiguration);
        }

        int retries = 0;

        while (retries < RetryUtilities.MaxRetries)
        {
            try
            {
                using var client = clientFactory.CreateClient();

                var result = await client.PostAsJsonAsync(this.configuration.BaseUrl, new List<TweetDto> { message }, cancellationToken);

                if (result is null || !result.IsSuccessStatusCode)
                {
                    this.logger.LogError(ProducerLogMessages.ProducerError, message.Id, message);
                }
                else
                {
                    this.logger.LogDebug(ProducerLogMessages.Success, DateTime.Now, result.StatusCode, message.Id);

                    return;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ProducerLogMessages.ProducerException, ex.Message);

                // Exit loop if exception is not transient
                if (!RetryUtilities.ExceptionIsTransient(ex))
                {
                    this.hasFaulted = true;

                    return;
                }
            }
        }

        this.hasFaulted = retries >= RetryUtilities.MaxRetries;
    }
}