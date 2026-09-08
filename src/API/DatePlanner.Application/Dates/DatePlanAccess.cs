using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;

namespace DatePlanner.Application.Dates;

internal static class DatePlanAccess
{
    internal static async Task<Result<DatePlan>> GetForUserAsync(
        Guid datePlanId,
        Guid userId,
        IDatePlanRepository datePlanRepository,
        ICoupleRepository coupleRepository)
    {
        DatePlan? datePlan = await datePlanRepository.GetDateByIdAsync(datePlanId);

        if (datePlan is null)
        {
            return Result.Failure<DatePlan>(DatePlanErrors.NotFound(datePlanId));
        }

        Couple? couple = await coupleRepository.GetCoupleByIdAsync(datePlan.CoupleId);
        bool isMember = couple is not null &&
                        (couple.PartnerOneId == userId || couple.PartnerTwoId == userId);

        return isMember
            ? datePlan
            : Result.Failure<DatePlan>(DatePlanErrors.NotFound(datePlanId));
    }
}
