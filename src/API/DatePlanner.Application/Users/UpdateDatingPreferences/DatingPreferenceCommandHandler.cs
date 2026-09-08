using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Users;
using MediatR;

namespace DatePlanner.Application.Users.UpdateDatingPreferences;

public sealed class DatingPreferenceCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) :
    IRequestHandler<AddFavoriteActivityCommand, Result>,
    IRequestHandler<RemoveFavoriteActivityCommand, Result>,
    IRequestHandler<AddAvoidedActivityCommand, Result>,
    IRequestHandler<RemoveAvoidedActivityCommand, Result>,
    IRequestHandler<AddFoodPreferenceCommand, Result>,
    IRequestHandler<RemoveFoodPreferenceCommand, Result>
{
    public Task<Result> Handle(AddFavoriteActivityCommand request, CancellationToken cancellationToken) =>
        Update(request.UserId, preferences => preferences.AddFavoriteActivity(request.Activity.Trim()), cancellationToken);

    public Task<Result> Handle(RemoveFavoriteActivityCommand request, CancellationToken cancellationToken) =>
        Update(request.UserId, preferences => preferences.RemoveFavoriteActivity(request.Activity.Trim()), cancellationToken);

    public Task<Result> Handle(AddAvoidedActivityCommand request, CancellationToken cancellationToken) =>
        Update(request.UserId, preferences => preferences.AddAvoidedActivity(request.Activity.Trim()), cancellationToken);

    public Task<Result> Handle(RemoveAvoidedActivityCommand request, CancellationToken cancellationToken) =>
        Update(request.UserId, preferences => preferences.RemoveAvoidedActivity(request.Activity.Trim()), cancellationToken);

    public Task<Result> Handle(AddFoodPreferenceCommand request, CancellationToken cancellationToken) =>
        Update(request.UserId, preferences => preferences.AddFoodPreferences(request.Food.Trim()), cancellationToken);

    public Task<Result> Handle(RemoveFoodPreferenceCommand request, CancellationToken cancellationToken) =>
        Update(request.UserId, preferences => preferences.RemoveFoodPreferences(request.Food.Trim()), cancellationToken);

    private async Task<Result> Update(
        Guid userId,
        Func<DatingPreferences, Result> update,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetUserByIdAsync(userId);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }

        Result result = update(user.GetOrCreateDatingPreferences());

        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
