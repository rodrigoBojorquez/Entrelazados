using FluentValidation;

namespace DatePlanner.Application.Couples.EndRelationship;

internal sealed class EndRelationshipCommandValidator : AbstractValidator<EndRelationshipCommand>
{
    public EndRelationshipCommandValidator() => RuleFor(command => command.UserId).NotEmpty();
}
