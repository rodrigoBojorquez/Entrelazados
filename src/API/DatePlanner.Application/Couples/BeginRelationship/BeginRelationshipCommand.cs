using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.Couples.BeginRelationship;

public sealed record BeginRelationshipCommand(Guid UserId, string PartnerCode) : IRequest<Result>;

internal sealed class BeginRelationshipCommandValidator : AbstractValidator<BeginRelationshipCommand>
{
    public BeginRelationshipCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.PartnerCode).NotEmpty().Length(12);
    }
}
