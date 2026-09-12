using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Dates.ReturnDatePlanToPlanning;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Dates;

internal sealed class ReturnDatePlanToPlanningEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/dates/{datePlanId:guid}/planning", async (
                Guid datePlanId,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new ReturnDatePlanToPlanningCommand(
                        context.User.GetUserId(),
                        datePlanId),
                    cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Dates);
    }
}
