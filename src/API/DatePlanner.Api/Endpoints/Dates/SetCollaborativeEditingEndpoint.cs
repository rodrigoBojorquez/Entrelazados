using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Dates.SetCollaborativeEditing;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Dates;

internal sealed class SetCollaborativeEditingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/dates/{datePlanId:guid}/collaboration", async (
                Guid datePlanId,
                Request request,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new SetCollaborativeEditingCommand(
                    context.User.GetUserId(),
                    datePlanId,
                    request.IsCollaborative);
                var result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Dates);
    }

    internal sealed record Request(bool IsCollaborative);
}
