using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using MediatR;

namespace DatePlanner.Application.Dates.RemoveItineraryItem;

public sealed class RemoveItineraryItemCommandHandler(
    IDatePlanRepository datePlanRepository,
    ICoupleRepository coupleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveItineraryItemCommand, Result>
{
    public async Task<Result> Handle(
        RemoveItineraryItemCommand request,
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

        Result result = accessResult.Value.RemoveItineraryItem(request.ItineraryItemId);

        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
