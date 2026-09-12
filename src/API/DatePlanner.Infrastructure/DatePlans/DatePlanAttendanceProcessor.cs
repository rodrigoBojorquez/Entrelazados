using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Dates;

namespace DatePlanner.Infrastructure.DatePlans;

public sealed class DatePlanAttendanceProcessor(
    IDatePlanRepository datePlanRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    public async Task<int> ProcessAsync(CancellationToken cancellationToken)
    {
        DateOnly today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        IReadOnlyCollection<DatePlan> plans =
            await datePlanRepository.GetBookedBeforeAsync(today);

        foreach (DatePlan plan in plans)
        {
            plan.Attend();
        }

        if (plans.Count > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return plans.Count;
    }
}
