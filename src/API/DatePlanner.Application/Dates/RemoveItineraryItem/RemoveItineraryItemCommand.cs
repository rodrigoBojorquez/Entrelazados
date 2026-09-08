using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.Dates.RemoveItineraryItem;

public sealed record RemoveItineraryItemCommand(
    Guid UserId,
    Guid DatePlanId,
    Guid ItineraryItemId) : IRequest<Result>;

internal sealed class RemoveItineraryItemCommandValidator : AbstractValidator<RemoveItineraryItemCommand>
{
    public RemoveItineraryItemCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DatePlanId).NotEmpty();
        RuleFor(command => command.ItineraryItemId).NotEmpty();
    }
}
