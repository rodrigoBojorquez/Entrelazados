using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.Dates.AddItineraryItem;

public sealed record AddItineraryItemCommand(
    Guid UserId,
    Guid DatePlanId,
    string Name,
    decimal? Latitude,
    decimal? Longitude,
    TimeOnly? StartsAt) : IRequest<Result>;

internal sealed class AddItineraryItemCommandValidator : AbstractValidator<AddItineraryItemCommand>
{
    public AddItineraryItemCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DatePlanId).NotEmpty();
        RuleFor(command => command.Name).NotEmpty().MaximumLength(200);

        RuleFor(command => command)
            .Must(command => command.Latitude.HasValue == command.Longitude.HasValue)
            .WithErrorCode("DatePlan.IncompleteLocation")
            .WithMessage("La latitud y la longitud deben proporcionarse juntas.");
    }
}
