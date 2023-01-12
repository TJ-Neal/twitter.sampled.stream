using Microsoft.AspNetCore.Mvc;
using Neal.Twitter.Application.Constants.Api;
using Neal.Twitter.Application.Interfaces.TweetRepository;
using Neal.Twitter.Core.Entities.Configuration;
using Neal.Twitter.Core.Entities.Twitter;

namespace Neal.Twitter.API.Kafka.Tweet.Endpoints;

/// <summary>
/// Represents endpoints for this API to interact with the underlying repository.
/// This repository uses the Kafka technologies to accomplish a message bus architecture while maintaining a performant, scalable, and durable design
/// </summary>
public static class RepositoryEndpoints
{
    public static void MapRepositoryEndpoints(this IEndpointRouteBuilder routes, string groupTag)
    {
        var group = routes.MapGroup(ApiStrings.BaseRoute)
            .WithTags(groupTag);

        group.MapGet(ApiStrings.TweetsRoute,
                ([FromServices] ITweetRepository model, [FromQuery] int? page, [FromQuery] int? pageSize) => model.GetAllTweetsAsync(new Pagination(page, pageSize)))
            .WithName(ApiStrings.GetTweetsName)
            .WithDescription(ApiStrings.GetTweetsDescription)
            .WithOpenApi();

        group.MapPost(ApiStrings.TweetsRoute, ([FromServices] ITweetRepository model, [FromBody] List<TweetDto> tweets) => model.AddRecordsAsync(tweets))
            .WithName(ApiStrings.PostTweetsName)
            .WithDescription(ApiStrings.PostTweetsDescription);

        group.MapGet(ApiStrings.CountRoute, ([FromServices] ITweetRepository model) => model.GetCountAsync())
            .WithName(ApiStrings.GetCountName)
            .WithDescription(ApiStrings.GetCountDescription)
            .WithOpenApi();

        group.MapGet(ApiStrings.HashtagsRoute, ([FromServices] ITweetRepository model, [FromQuery] int? top) => model.GetTopHashtags(top ?? 10))
            .WithName(ApiStrings.GetHashtagsName)
            .WithDescription(ApiStrings.GetHashtagsDescription)
            .WithOpenApi();
    }
}