using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Couples.GetCurrentPartner;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Couples;

internal sealed class GetCurrentPartnerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/couples/current/partner", async (
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var userId = context.User.GetUserId();
                var result = await sender.Send(new GetCurrentPartnerQuery(userId), cancellationToken);

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Couples);
    }
}