using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Dates.ListDatePlans;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Dates;

internal sealed class ListDatePlansEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dates", async (
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new ListDatePlansQuery(context.User.GetUserId());
                var result = await sender.Send(query, cancellationToken);

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Dates);
    }
}
