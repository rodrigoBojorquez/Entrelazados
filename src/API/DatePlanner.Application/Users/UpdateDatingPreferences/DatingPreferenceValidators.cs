using FluentValidation;

namespace DatePlanner.Application.Users.UpdateDatingPreferences;

internal abstract class DatingPreferenceCommandValidator<TCommand> : AbstractValidator<TCommand>
{
    protected DatingPreferenceCommandValidator(
        Func<TCommand, Guid> userId,
        Func<TCommand, string> value)
    {
        RuleFor(command => userId(command)).NotEmpty();
        RuleFor(command => value(command)).NotEmpty().MaximumLength(200);
    }
}

internal sealed class AddFavoriteActivityCommandValidator()
    : DatingPreferenceCommandValidator<AddFavoriteActivityCommand>(x => x.UserId, x => x.Activity);

internal sealed class RemoveFavoriteActivityCommandValidator()
    : DatingPreferenceCommandValidator<RemoveFavoriteActivityCommand>(x => x.UserId, x => x.Activity);

internal sealed class AddAvoidedActivityCommandValidator()
    : DatingPreferenceCommandValidator<AddAvoidedActivityCommand>(x => x.UserId, x => x.Activity);

internal sealed class RemoveAvoidedActivityCommandValidator()
    : DatingPreferenceCommandValidator<RemoveAvoidedActivityCommand>(x => x.UserId, x => x.Activity);

internal sealed class AddFoodPreferenceCommandValidator()
    : DatingPreferenceCommandValidator<AddFoodPreferenceCommand>(x => x.UserId, x => x.Food);

internal sealed class RemoveFoodPreferenceCommandValidator()
    : DatingPreferenceCommandValidator<RemoveFoodPreferenceCommand>(x => x.UserId, x => x.Food);