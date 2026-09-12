using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.TurnGames.GetCurrentScheduledTurnGame;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.TurnGames;

internal sealed class GetCurrentScheduledTurnGameEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/turn-games/current-scheduled", async (
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetCurrentScheduledTurnGameQuery(context.User.GetUserId());
                var result = await sender.Send(query, cancellationToken);

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.TurnGames);
    }
}
