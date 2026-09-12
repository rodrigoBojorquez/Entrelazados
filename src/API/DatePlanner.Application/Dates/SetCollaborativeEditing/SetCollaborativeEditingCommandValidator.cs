using FluentValidation;

namespace DatePlanner.Application.Dates.SetCollaborativeEditing;

internal sealed class SetCollaborativeEditingCommandValidator
    : AbstractValidator<SetCollaborativeEditingCommand>
{
    public SetCollaborativeEditingCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DatePlanId).NotEmpty();
    }
}
