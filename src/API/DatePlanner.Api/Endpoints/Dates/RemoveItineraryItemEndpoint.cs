using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Dates.RemoveItineraryItem;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Dates;

internal sealed class RemoveItineraryItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/dates/{datePlanId:guid}/itinerary/{itineraryItemId:guid}", async (
                Guid datePlanId,
                Guid itineraryItemId,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new RemoveItineraryItemCommand(
                    context.User.GetUserId(),
                    datePlanId,
                    itineraryItemId);
                var result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Dates);
    }
}
