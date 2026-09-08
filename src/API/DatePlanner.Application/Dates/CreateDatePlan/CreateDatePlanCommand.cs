using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.Dates.CreateDatePlan;

public sealed record CreateDatePlanCommand(Guid UserId) : IRequest<Result<Guid>>;

internal sealed class CreateDatePlanCommandValidator : AbstractValidator<CreateDatePlanCommand>
{
    public CreateDatePlanCommandValidator() => RuleFor(command => command.UserId).NotEmpty();
}
