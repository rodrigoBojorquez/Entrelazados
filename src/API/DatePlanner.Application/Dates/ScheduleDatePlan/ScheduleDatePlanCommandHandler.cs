using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using MediatR;

namespace DatePlanner.Application.Dates.ScheduleDatePlan;

public sealed class ScheduleDatePlanCommandHandler(
    IDatePlanRepository datePlanRepository,
    ICoupleRepository coupleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ScheduleDatePlanCommand, Result>
{
    public async Task<Result> Handle(
        ScheduleDatePlanCommand request,
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

        Result result = accessResult.Value.Schedule(request.ScheduledFor);

        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
