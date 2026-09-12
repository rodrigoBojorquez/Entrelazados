using DatePlanner.Domain.Dates;
using DatePlanner.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DatePlanner.Infrastructure.DatePlans;

internal sealed class DatePlanRepository(DatePlannerDbContext dbContext) : IDatePlanRepository
{
    public Task<DatePlan?> GetDateByIdAsync(Guid dateId) =>
        dbContext.DatePlans
            .Include(datePlan => datePlan.Itinerary)
            .FirstOrDefaultAsync(datePlan => datePlan.Id == dateId);

    public async Task<IReadOnlyCollection<DatePlan>> GetBookedBeforeAsync(DateOnly date) =>
        await dbContext.DatePlans
            .Where(datePlan =>
                datePlan.Status == DateStatus.Booked &&
                datePlan.ScheduledFor < date)
            .ToListAsync();

    public void Insert(DatePlan datePlan) => dbContext.DatePlans.Add(datePlan);
}
