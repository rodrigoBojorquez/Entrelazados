using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Authentication.RefreshToken;
using MediatR;

namespace DatePlanner.Api.Endpoints.Authentication;

internal sealed class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/authentication/refresh", async (
                Request request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new RefreshTokenCommand(request.RefreshToken);
                var result = await sender.Send(command, cancellationToken);

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .AllowAnonymous()
            .WithTags(Tags.Authentication);
    }

    internal sealed record Request(string RefreshToken);
}
