using LinqToTwitter;
using MediatR;

namespace Neal.Twitter.Application.Events.Notifications;

public record TweetReceivedNotification(Tweet Tweet, string Topic) : INotification;