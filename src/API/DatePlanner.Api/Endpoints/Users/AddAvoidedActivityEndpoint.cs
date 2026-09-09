using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Results;
using DatePlanner.Application.Users.UpdateDatingPreferences;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;

namespace DatePlanner.Api.Endpoints.Users;

internal sealed class AddAvoidedActivityEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/me/dating-preferences/avoided-activities", async (
                Request request, HttpContext context, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(
                new AddAvoidedActivityCommand(context.User.GetUserId(), request.Activity),
                cancellationToken)).Match(Results.NoContent, ApiResults.Problem))
            .RequireAuthorization()
            .WithTags(Tags.Users);
    }

    internal sealed record Request(string Activity);
}
