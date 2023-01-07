using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants.Keys;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Utilities;
using Neal.Twitter.Core.Entities.Twitter;
using Neal.Twitter.Simple.Client.Constants;
using Neal.Twitter.Simple.Client.Interfaces;
using System.Net.Http.Json;

namespace Neal.Twitter.Simple.Client.Wrappers;

public sealed class SimpleProducerWrapper : ISimpleProducerWrapper
{
    #region Fields

    private readonly IConfiguration configuration;

    private readonly ILogger<SimpleProducerWrapper> logger;

    private readonly IHttpClientFactory clientFactory;

    private bool hasFaulted = false;

    #endregion Fields

    public SimpleProducerWrapper(
        IConfiguration configuration,
        ILogger<SimpleProducerWrapper> logger,
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

        // TODO: Use a model for this
        string baseUrl = this.configuration
            .GetSection(ApplicationConfigurationKeys.Simple)
            .GetValue<string>(ApplicationConfigurationKeys.BaseUrl)
                ?? string.Empty;

        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new InvalidOperationException(CommonLogMessages.InvalidConfiguration);
        }

        int retries = 0;

        while (retries < RetryUtilities.MaxRetries)
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
                    this.logger.LogInformation(ProducerLogMessages.Success, DateTime.Now, result.StatusCode, message.Id);

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