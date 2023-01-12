using MediatR;
using Neal.Twitter.Core.Entities.Twitter;

namespace Neal.Twitter.Application.Events.Notifications;

/// <summary>
/// Represents a notification for when a distinct tweet has been received from the Twitter client stream.
/// </summary>
/// <param name="Tweet"></param>
public record TweetReceivedNotification(TweetDto Tweet) : INotification;