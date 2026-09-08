using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.Users.UpdateDatingPreferences;

public sealed record AddFavoriteActivityCommand(Guid UserId, string Activity) : IRequest<Result>;

public sealed record RemoveFavoriteActivityCommand(Guid UserId, string Activity) : IRequest<Result>;

public sealed record AddAvoidedActivityCommand(Guid UserId, string Activity) : IRequest<Result>;

public sealed record RemoveAvoidedActivityCommand(Guid UserId, string Activity) : IRequest<Result>;

public sealed record AddFoodPreferenceCommand(Guid UserId, string Food) : IRequest<Result>;

public sealed record RemoveFoodPreferenceCommand(Guid UserId, string Food) : IRequest<Result>;

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
