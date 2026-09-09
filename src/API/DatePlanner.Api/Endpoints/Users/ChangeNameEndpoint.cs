using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Users.ChangeName;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Users;

internal sealed class ChangeNameEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/me/name", async (
                Request request,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new ChangeNameCommand(
                    context.User.GetUserId(),
                    request.FirstName,
                    request.LastName);
                var result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Users);
    }

    internal sealed record Request(string FirstName, string LastName);
}
