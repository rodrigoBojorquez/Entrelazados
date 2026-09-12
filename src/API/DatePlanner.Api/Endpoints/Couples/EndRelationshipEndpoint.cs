using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Couples.EndRelationship;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Couples;

internal sealed class EndRelationshipEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/couples/current", async (
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new EndRelationshipCommand(context.User.GetUserId());
                var result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Couples);
    }
}
