using Microsoft.AspNetCore.Mvc;
using Neal.Twitter.Application.Constants.Api;
using Neal.Twitter.Application.Interfaces.TweetRepository;

namespace Neal.Twitter.Infrastructure.Kafka.Tweet.API.Endpoints;

public static class RepositoryEndpoints
{
    public static void MapRepositoryEndpoints(this IEndpointRouteBuilder routes, string groupTag)
    {
        var group = routes.MapGroup(ApiStrings.BaseRoute)
            .WithTags(groupTag);

        group.MapGet(ApiStrings.TweetsRoute, ([FromServices] ITweetRepository model, [FromQuery] int? page, [FromQuery] int? pagesize) => model.GetAllTweetsAsync(page, pagesize))
            .WithName(ApiStrings.GetTweetsName)
            .WithDescription(ApiStrings.GetTweetsDescription)
            .WithOpenApi();

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