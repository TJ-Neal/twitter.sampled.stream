using Confluent.Kafka;
using LinqToTwitter;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Application.UnitTests.TestFixtures;

namespace Neal.Twitter.Application.UnitTests.Events.Handlers;

public class TweetReceivedKafkaHandlerTests : IClassFixture<TweetReceivedKafkaHandlerFixture>
{
    #region Fields

    private readonly TweetReceivedKafkaHandlerFixture _tweetReceivedKafkaHandlerFixture;

    #endregion Fields

    public TweetReceivedKafkaHandlerTests(TweetReceivedKafkaHandlerFixture tweetReceivedKafkaHandlerFixture) =>
        this._tweetReceivedKafkaHandlerFixture = tweetReceivedKafkaHandlerFixture;

    [Fact]
    public async Task TweetReceived_WithTokenCancelled_DoesNotInvoke_Handle()
    {
        // Arrange
        string expectedException = nameof(this._tweetReceivedKafkaHandlerFixture.MockKafkaProducerWrapper.Produce);
        var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;

        // Act
        Assert.NotNull(this._tweetReceivedKafkaHandlerFixture.MockMediator); // This should never be null due to fixture

        cancellationSource.Cancel();

        // Publish a fake TweetReceivedNotification with canceled token. Should not invoke wrapper produce method since token is already canceled.
        await this._tweetReceivedKafkaHandlerFixture
            .MockMediator
            .Publish(new TweetReceivedNotification(GenerateFakeTweet(), string.Empty), cancellationToken);

        // Verify that the mediator did not cause a call to the wrapper to be invoked because the token was already canceled.
        var verifyAll = () => Mock.Get(this._tweetReceivedKafkaHandlerFixture.MockKafkaProducerWrapper)
            .VerifyAll();

        // Assert
        var exception = Assert.Throws<MockException>(verifyAll);    // Verification should fail
        Assert.True(cancellationSource.IsCancellationRequested);    // Token was canceled as expected
        Assert.True(exception.IsVerificationError);                 // Needs to be a verification exception
        Assert.Contains(expectedException, exception.Message);      // Needs to match expected UnmatchedSetup, Publish
    }

    [Fact]
    public async Task PublishTweetReceived_WithCancellationToken_Causes_IsCanceled()
    {
        // Arrange
        bool callbackCalled = false;
        var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;
        var publishCallback = delegate ()
        {
            cancellationSource.Cancel();
            callbackCalled = true;

            return Task.CompletedTask;
        };

        // Attach Xunit setup to mock callback in wrapper so we know that the mediator calls the wrapper handler
        Mock.Get(this._tweetReceivedKafkaHandlerFixture.MockKafkaProducerWrapper)
            .Setup(wrapper => wrapper.Produce(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
            .Returns(publishCallback());

        // Act
        Assert.NotNull(this._tweetReceivedKafkaHandlerFixture.MockMediator); // This should never be null due to fixture

        // Publish a fake TweetReceivedNotification so the mocked handler can be verified to have called the callback function
        await this._tweetReceivedKafkaHandlerFixture
            .MockMediator
            .Publish(new TweetReceivedNotification(GenerateFakeTweet(), string.Empty), cancellationToken);

        // Assert
        // Confirm that the callback mock was called for the handler
        Assert.True(callbackCalled);
        Assert.True(cancellationToken.IsCancellationRequested);
    }

    #region Private methods

    private static Tweet GenerateFakeTweet() => new()
    {
        Text = "This is a test tweet",
        Entities = new TweetEntities
        {
            Hashtags = new List<TweetEntityHashtag> {
                        new TweetEntityHashtag
                        {
                            Start = 10,
                            End = 20,
                            Tag = "TEST"
                        }
                    }
        }
    };

    #endregion Private methods
}