using DatePlanner.Api.Common.Endpoints;

namespace DatePlanner.Api.Endpoints.TurnGames;

internal sealed class TurnGameHubEndpoint : IEndpoint
{
    internal const string Route = "/hubs/turn-games";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapHub<TurnGameHub>(Route)
            .RequireAuthorization()
            .WithTags(Tags.TurnGames);
    }
}
