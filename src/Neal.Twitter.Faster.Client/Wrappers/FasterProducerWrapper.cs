using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants.Keys;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Utilities;
using Neal.Twitter.Core.Entities.Twitter;
using Neal.Twitter.Faster.Client.Constants;
using Neal.Twitter.Faster.Client.Interfaces;
using System.Net.Http.Json;

namespace Neal.Twitter.Faster.Client.Wrappers;

public sealed class FasterProducerWrapper : IFasterProducerWrapper
{
    #region Fields

    private readonly IConfiguration configuration;

    private readonly ILogger<FasterProducerWrapper> logger;

    private readonly IHttpClientFactory clientFactory;

    private bool hasFaulted = false;

    #endregion Fields

    public FasterProducerWrapper(
        IConfiguration configuration,
        ILogger<FasterProducerWrapper> logger,
        IHttpClientFactory clientFactory)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.clientFactory = clientFactory;
    }

    public async Task ProduceAsync(TweetDto message, CancellationToken cancellationToken)
    {
        if (hasFaulted
            || cancellationToken.IsCancellationRequested)
        {
            return;
        }

        // TODO: Use faster model
        string baseUrl = this.configuration
            .GetSection(ApplicationConfigurationKeys.Faster)
            .GetValue<string>(ApplicationConfigurationKeys.BaseUrl)
                ?? string.Empty;

        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new InvalidOperationException(CommonLogMessages.InvalidConfiguration);
        }

        int retries = 0;

        while (retries < 10)
        {
            try
            {
                using var client = clientFactory.CreateClient();

                var result = await client.PostAsJsonAsync(baseUrl, new List<TweetDto> { message }, cancellationToken);

                if (result is null || !result.IsSuccessStatusCode)
                {
                    this.logger.LogError(ProducerLogMessages.ProducerError, message.Id, message);
                }
                else
                {
                    this.logger.LogInformation(ProducerLogMessages.PrintResult, DateTime.Now, result.StatusCode, message.Id);

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

            await Task.Delay(RetryUtilities.GetDelay(++retries), cancellationToken);
        }

        this.hasFaulted = retries >= RetryUtilities.MaxRetries;
    }
}