using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Couples.BeginRelationship;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Couples;

internal sealed class BeginRelationshipEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/couples", async (
                Request request,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new BeginRelationshipCommand(
                    context.User.GetUserId(),
                    request.PartnerCode);
                var result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Couples);
    }

    internal sealed record Request(string PartnerCode);
}
