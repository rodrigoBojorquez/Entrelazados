namespace DatePlanner.Domain.Dates;

public interface IDatePlanRepository
{
    Task<DatePlan?> GetDateByIdAsync(Guid dateId);

    Task<IReadOnlyCollection<DatePlan>> GetBookedBeforeAsync(DateOnly date);
    
    void Insert(DatePlan datePlan);
}
