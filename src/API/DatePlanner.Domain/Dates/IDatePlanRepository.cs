namespace DatePlanner.Domain.Dates;

public interface IDatePlanRepository
{
    Task<DatePlan?> GetDateByIdAsync(Guid dateId);
    
    void Insert(DatePlan datePlan);
}