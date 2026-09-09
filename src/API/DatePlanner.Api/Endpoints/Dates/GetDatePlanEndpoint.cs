using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Dates.GetDatePlan;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Dates;

internal sealed class GetDatePlanEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dates/{datePlanId:guid}", async (
                Guid datePlanId,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetDatePlanQuery(context.User.GetUserId(), datePlanId);
                var result = await sender.Send(query, cancellationToken);

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Dates);
    }
}
