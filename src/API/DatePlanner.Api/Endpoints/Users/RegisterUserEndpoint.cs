using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Users.RegisterUser;
using MediatR;

namespace DatePlanner.Api.Endpoints.Users;

internal sealed class RegisterUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users", async (
                Request request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new RegisterUserCommand(
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.Password);
                var result = await sender.Send(command, cancellationToken);

                return result.Match(
                    id => Results.Created($"/api/users/{id}", new { id }),
                    ApiResults.Problem);
            })
            .AllowAnonymous()
            .WithTags(Tags.Users);
    }

    internal sealed record Request(
        string Email,
        string FirstName,
        string LastName,
        string Password);
}
