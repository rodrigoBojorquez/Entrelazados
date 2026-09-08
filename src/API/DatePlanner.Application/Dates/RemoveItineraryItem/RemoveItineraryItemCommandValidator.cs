using FluentValidation;

namespace DatePlanner.Application.Dates.RemoveItineraryItem;

internal sealed class RemoveItineraryItemCommandValidator : AbstractValidator<RemoveItineraryItemCommand>
{
    public RemoveItineraryItemCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DatePlanId).NotEmpty();
        RuleFor(command => command.ItineraryItemId).NotEmpty();
    }
}