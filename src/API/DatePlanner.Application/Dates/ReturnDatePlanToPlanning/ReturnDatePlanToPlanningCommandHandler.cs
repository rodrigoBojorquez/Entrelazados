using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using MediatR;

namespace DatePlanner.Application.Dates.ReturnDatePlanToPlanning;

public sealed class ReturnDatePlanToPlanningCommandHandler(
    IDatePlanRepository datePlanRepository,
    ICoupleRepository coupleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ReturnDatePlanToPlanningCommand, Result>
{
    public async Task<Result> Handle(
        ReturnDatePlanToPlanningCommand request,
        CancellationToken cancellationToken)
    {
        Result<DatePlan> accessResult = await DatePlanAccess.GetForEditorAsync(
            request.DatePlanId,
            request.UserId,
            datePlanRepository,
            coupleRepository);

        if (accessResult.IsFailure)
        {
            return Result.Failure(accessResult.Error);
        }

        Result result = accessResult.Value.ReturnToPlanning();

        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
