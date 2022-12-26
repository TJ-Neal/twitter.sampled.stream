using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.OAuth;
using MediatR;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Application.Interfaces.LinqToTwitter;
using StreamContent = LinqToTwitter.StreamContent;

namespace Neal.Twitter.Application.Wrappers.LinqToTwitter;

public class LinqToTwitterWrapper : ILinqToTwitterWrapper
{
    #region Fields

    private readonly ILogger<LinqToTwitterWrapper> _logger;

    private readonly IAuthorizer _authorizer;

    private readonly IMediator _mediator;

    #endregion Fields

    #region Public Constructors

    public LinqToTwitterWrapper(
        ILogger<LinqToTwitterWrapper> logger,
        IMediator mediator,
        IAuthorizer authorizer)
    {
        this._logger = logger;
        this._mediator = mediator;
        this._authorizer = authorizer;
    }

    #endregion Public Constructors

    #region ILinqToTwitterWrapper implementation

    public virtual async Task StartAsync(Func<StreamContent, Task> callback, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            this._logger.LogInformation(CommonLogMessages.CancelRequested);
            return;
        }

        try
        {
            await this._authorizer.AuthorizeAsync();

            var twitterContext = new TwitterContext(this._authorizer);

            await twitterContext
                .Streaming
                .WithCancellation(cancellationToken)
                .Where(stream => stream.Type == StreamingType.Sample
                    && stream.TweetFields == string.Join(',', TweetField.Entities))
                .StartAsync(callback);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            this._logger.LogInformation(CommonLogMessages.CancelRequested); // Expected
        }
        catch (OperationCanceledException canceledException)
        {
            this._logger.LogCritical(canceledException, CommonLogMessages.CanceledException);
        }
        catch (IOException ioException)
        {
            this._logger.LogCritical(ioException, SampledStreamLogMessages.IOException);
            throw;
        }
        catch (Exception ex)
        {
            this._logger.LogCritical(ex, SampledStreamLogMessages.FatalException);
            throw;
        }
    }

    public async Task ProcessStreamContent(
        StreamContent stream,
        string topic,
        CancellationToken cancellationToken)
    {
        try
        {
            string streamContent = stream.Content;

            if (stream.HasError)
            {
                this._logger.LogWarning(SampledStreamLogMessages.StreamError, stream.ErrorMessage);
                return;
            }

            if (string.IsNullOrWhiteSpace(streamContent))
            {
                this._logger.LogInformation(SampledStreamLogMessages.KeepAlive);
                return;
            }

            var tweet = stream.Entity?.Tweet;

            if (tweet is null)
            {
                this._logger.LogWarning(SampledStreamLogMessages.NullTweet);
                return;
            }

            // Publish notification for tweet received
            await this._mediator
                .Publish(new TweetReceivedNotification(tweet, topic), cancellationToken);
        }
        catch (OperationCanceledException) { /* expected */ }
    }

    #endregion ILinqToTwitterWrapper implementation
}