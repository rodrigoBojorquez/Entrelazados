using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Authentication.Login;
using MediatR;

namespace DatePlanner.Api.Endpoints.Authentication;

internal sealed class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/authentication/login", async (
                Request request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new LoginCommand(request.Email, request.Password);
                var result = await sender.Send(command, cancellationToken);

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .AllowAnonymous()
            .WithTags(Tags.Authentication);
    }

    internal sealed record Request(string Email, string Password);
}
