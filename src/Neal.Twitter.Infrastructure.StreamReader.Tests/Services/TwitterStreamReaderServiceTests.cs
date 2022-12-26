﻿using LinqToTwitter.Provider;
using Moq;
using Neal.Twitter.Infrastructure.StreamReader.UnitTests.TestFixtures;

namespace Neal.Twitter.Infrastructure.StreamReader.UnitTests.Services;

public class TwitterStreamReaderServiceTests : IClassFixture<TwitterStreamReaderFixture>
{
    #region Fields

    private readonly TwitterStreamReaderFixture _twitterStreamReaderFixture;

    #endregion Fields

    public TwitterStreamReaderServiceTests(TwitterStreamReaderFixture twitterStreamReaderFixture) =>
        this._twitterStreamReaderFixture = twitterStreamReaderFixture;

    [Fact]
    public async Task Execute_BackgroundProcess_WithTokenCancelled_DoesNotInvoke_StartAsync()
    {
        // Arrange
        string expectedException = nameof(this._twitterStreamReaderFixture.MockLinqToTwitterWrapper.StartAsync);
        var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;

        Mock.Get(this._twitterStreamReaderFixture.MockLinqToTwitterWrapper)
            .Setup(wrapper => wrapper!.StartAsync(It.IsAny<Func<LinqToTwitter.StreamContent, Task>>(), It.IsAny<CancellationToken>()));

        // Act
        Assert.NotNull(this._twitterStreamReaderFixture.HostedService); // This should never be null due to fixture

        cancellationSource.Cancel();

        // Start the mocked TwitterStreamReaderService as a HostedService with an already canceled token. Should not invoke wrapper StartAsync method.
        await this._twitterStreamReaderFixture
            .HostedService
            .StartAsync(cancellationToken);

        // Verify that the wrapper StartAsync was not called due to canceled token.
        var verifyAll = () => Mock.Get(this._twitterStreamReaderFixture.MockLinqToTwitterWrapper)
            .VerifyAll();

        // Assert
        var exception = Assert.Throws<MockException>(verifyAll);    // Verification should fail
        Assert.True(cancellationSource.IsCancellationRequested);    // Token was canceled as expected
        Assert.True(exception.IsVerificationError);                 // Needs to be a verification exception
        Assert.Contains(expectedException, exception.Message);      // Needs to match expected UnmatchedSetup, StartAsync
    }

    [Fact]
    public async Task Execute_BackgroundProcess_WithCancellationToken_Causes_IsCanceled()
    {
        // Arrange
        bool callbackCalled = false;
        var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;
        var startAsyncCallback = delegate (LinqToTwitter.StreamContent _)
        {
            cancellationSource.Cancel();
            callbackCalled = true;

            return Task.CompletedTask;
        };

        // Attach Xunit setup to mock callback in wrapper so we know that the BackgroundService calls the wrapper handler
        Mock.Get(this._twitterStreamReaderFixture.MockLinqToTwitterWrapper)
            .Setup(wrapper => wrapper!.StartAsync(It.IsAny<Func<LinqToTwitter.StreamContent, Task>>(), It.IsAny<CancellationToken>()))
            .Returns(startAsyncCallback(new LinqToTwitter.StreamContent(Mock.Of<ITwitterExecute>(), string.Empty)));

        // Act
        Assert.NotNull(this._twitterStreamReaderFixture.HostedService); // This should never be null due to fixture

        // Start the hosted service with an active token
        await this._twitterStreamReaderFixture
            .HostedService
            .StartAsync(cancellationToken);

        // Assert
        // Confirm that the callback mock was called for the BackgroundService
        Assert.True(callbackCalled);
        Assert.True(cancellationToken.IsCancellationRequested);
    }
}