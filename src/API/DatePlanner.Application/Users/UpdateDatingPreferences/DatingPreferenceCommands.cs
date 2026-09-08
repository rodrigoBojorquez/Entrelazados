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
