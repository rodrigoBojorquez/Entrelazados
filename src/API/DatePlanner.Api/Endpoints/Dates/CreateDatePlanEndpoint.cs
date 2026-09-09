using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Dates.CreateDatePlan;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Dates;

internal sealed class CreateDatePlanEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/dates", async (
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new CreateDatePlanCommand(context.User.GetUserId()),
                    cancellationToken);

                return result.Match(
                    id => Results.Created($"/api/dates/{id}", new { id }),
                    ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Dates);
    }
}
