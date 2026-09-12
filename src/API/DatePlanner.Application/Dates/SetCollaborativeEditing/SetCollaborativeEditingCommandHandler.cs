using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using MediatR;

namespace DatePlanner.Application.Dates.SetCollaborativeEditing;

public sealed class SetCollaborativeEditingCommandHandler(
    IDatePlanRepository datePlanRepository,
    ICoupleRepository coupleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SetCollaborativeEditingCommand, Result>
{
    public async Task<Result> Handle(
        SetCollaborativeEditingCommand request,
        CancellationToken cancellationToken)
    {
        Result<DatePlan> accessResult = await DatePlanAccess.GetForUserAsync(
            request.DatePlanId,
            request.UserId,
            datePlanRepository,
            coupleRepository);

        if (accessResult.IsFailure)
        {
            return Result.Failure(accessResult.Error);
        }

        DatePlan datePlan = accessResult.Value;
        if (datePlan.PartnerWhoPlannedId != request.UserId)
        {
            return Result.Failure(DatePlanErrors.OnlyPlannerCanEdit);
        }

        Result result = datePlan.SetCollaborativeEditing(request.IsCollaborative);
        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
