using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.Dates.AttendDatePlan;

public sealed record AttendDatePlanCommand(Guid UserId, Guid DatePlanId) : IRequest<Result>;

internal sealed class AttendDatePlanCommandValidator : AbstractValidator<AttendDatePlanCommand>
{
    public AttendDatePlanCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DatePlanId).NotEmpty();
    }
}
