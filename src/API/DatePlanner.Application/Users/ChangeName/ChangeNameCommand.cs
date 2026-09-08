using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.Users.ChangeName;

public sealed record ChangeNameCommand(
    Guid UserId,
    string FirstName,
    string LastName) : IRequest<Result>;

internal sealed class ChangeNameCommandValidator : AbstractValidator<ChangeNameCommand>
{
    public ChangeNameCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(command => command.LastName).NotEmpty().MaximumLength(100);
    }
}
