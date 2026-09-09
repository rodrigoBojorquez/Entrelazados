using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Authentication.RevokeRefreshToken;
using MediatR;

namespace DatePlanner.Api.Endpoints.Authentication;

internal sealed class RevokeRefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/authentication/revoke", async (
                Request request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new RevokeRefreshTokenCommand(request.RefreshToken),
                    cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .AllowAnonymous()
            .WithTags(Tags.Authentication);
    }

    internal sealed record Request(string RefreshToken);
}
