using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Dates.AddItineraryItem;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Dates;

internal sealed class AddItineraryItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/dates/{datePlanId:guid}/itinerary", async (
                Guid datePlanId,
                Request request,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new AddItineraryItemCommand(
                    context.User.GetUserId(),
                    datePlanId,
                    request.Name,
                    request.Latitude,
                    request.Longitude,
                    request.StartsAt);
                var result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Dates);
    }

    internal sealed record Request(
        string Name,
        decimal? Latitude,
        decimal? Longitude,
        TimeOnly? StartsAt);
}
