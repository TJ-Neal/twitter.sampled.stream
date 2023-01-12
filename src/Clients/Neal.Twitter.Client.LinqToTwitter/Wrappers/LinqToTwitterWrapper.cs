using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.OAuth;
using MediatR;
using Microsoft.Extensions.Logging;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Client.LinqToTwitter.Constants;
using Neal.Twitter.Client.LinqToTwitter.Interfaces;
using Neal.Twitter.Core.Entities.Twitter;
using StreamContent = LinqToTwitter.StreamContent;

namespace Neal.Twitter.Client.LinqToTwitter.Wrappers;

/// <summary>
/// <see cref="ILinqToTwitterWrapper"/>
/// </summary>
public class LinqToTwitterWrapper : ILinqToTwitterWrapper
{
    #region Fields

    private readonly ILogger<LinqToTwitterWrapper> logger;

    private readonly IAuthorizer authorizer;

    private readonly IMediator mediator;

    #endregion Fields

    #region Public Constructors

    public LinqToTwitterWrapper(
        ILogger<LinqToTwitterWrapper> logger,
        IMediator mediator,
        IAuthorizer authorizer)
    {
        this.logger = logger;
        this.mediator = mediator;
        this.authorizer = authorizer;
    }

    #endregion Public Constructors

    #region ILinqToTwitterWrapper implementation

    public virtual async Task StartAsync(Func<StreamContent, Task> callback, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            this.logger.LogInformation(CommonLogMessages.CancelRequested);
            return;
        }

        try
        {
            await this.authorizer.AuthorizeAsync();

            var twitterContext = new TwitterContext(this.authorizer);

            await twitterContext
                .Streaming
                .WithCancellation(cancellationToken)
                .Where(stream => stream.Type == StreamingType.Sample
                    && stream.TweetFields == string.Join(',', TweetField.Entities))
                .StartAsync(callback);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            this.logger.LogInformation(CommonLogMessages.CancelRequested); // Expected
        }
        catch (OperationCanceledException canceledException)
        {
            this.logger.LogCritical(canceledException, CommonLogMessages.CanceledException);
        }
        catch (IOException ioException)
        {
            this.logger.LogCritical(ioException, SampledStreamLogMessages.IOException);
            throw;
        }
        catch (Exception ex)
        {
            this.logger.LogCritical(ex, SampledStreamLogMessages.FatalException);
            throw;
        }
    }

    public Task ProcessStreamContent(
        StreamContent stream,
        CancellationToken cancellationToken)
    {
        try
        {
            string streamContent = stream.Content;

            if (stream.HasError)
            {
                this.logger.LogWarning(SampledStreamLogMessages.StreamError, stream.ErrorMessage);
                return Task.CompletedTask;
            }

            if (string.IsNullOrWhiteSpace(streamContent))
            {
                this.logger.LogInformation(SampledStreamLogMessages.KeepAlive);
                return Task.CompletedTask;
            }

            var tweet = stream.Entity?.Tweet;

            if (tweet is null)
            {
                this.logger.LogWarning(SampledStreamLogMessages.NullTweet);
                return Task.CompletedTask;
            }

            // Publish notification for tweet received
            this.mediator
                .Publish(new TweetReceivedNotification(new TweetDto(tweet)), cancellationToken);
        }
        catch (OperationCanceledException) { /* expected */ }

        return Task.CompletedTask;
    }

    #endregion ILinqToTwitterWrapper implementation
}