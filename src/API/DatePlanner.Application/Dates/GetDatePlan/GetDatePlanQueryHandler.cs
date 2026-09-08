using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using MediatR;

namespace DatePlanner.Application.Dates.GetDatePlan;

public sealed class GetDatePlanQueryHandler(
    IDatePlanRepository datePlanRepository,
    ICoupleRepository coupleRepository)
    : IRequestHandler<GetDatePlanQuery, Result<DatePlanResponse>>
{
    public async Task<Result<DatePlanResponse>> Handle(
        GetDatePlanQuery request,
        CancellationToken cancellationToken)
    {
        Result<DatePlan> accessResult = await DatePlanAccess.GetForUserAsync(
            request.DatePlanId,
            request.UserId,
            datePlanRepository,
            coupleRepository);

        if (accessResult.IsFailure)
        {
            return Result.Failure<DatePlanResponse>(accessResult.Error);
        }

        DatePlan datePlan = accessResult.Value;

        return new DatePlanResponse(
            datePlan.Id,
            datePlan.CoupleId,
            datePlan.PartnerWhoPlannedId,
            datePlan.ScheduledFor,
            datePlan.Status,
            datePlan.Itinerary
                .OrderBy(item => item.Order)
                .Select(item => new ItineraryItemResponse(
                    item.Id,
                    item.Order,
                    item.Name,
                    item.StartsAt,
                    item.Location?.Latitude,
                    item.Location?.Longitude))
                .ToArray());
    }
}
