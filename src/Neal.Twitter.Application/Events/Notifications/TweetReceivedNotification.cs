using MediatR;
using Neal.Twitter.Core.Entities.Twitter;

namespace Neal.Twitter.Application.Events.Notifications;

public record TweetReceivedNotification(TweetDto Tweet) : INotification;