using FluentValidation;

namespace DatePlanner.Application.Couples.BeginRelationship;

internal sealed class BeginRelationshipCommandValidator : AbstractValidator<BeginRelationshipCommand>
{
    public BeginRelationshipCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.PartnerCode).NotEmpty().Length(12);
    }
}