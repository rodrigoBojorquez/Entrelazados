using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.Dates.CancelDatePlan;

public sealed record CancelDatePlanCommand(Guid UserId, Guid DatePlanId) : IRequest<Result>;

internal sealed class CancelDatePlanCommandValidator : AbstractValidator<CancelDatePlanCommand>
{
    public CancelDatePlanCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DatePlanId).NotEmpty();
    }
}
